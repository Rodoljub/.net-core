using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        private QDbContext _context;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IServiceProvider Services { get; }
        public IBackgroundTaskQueue Queue { get; }

        public EventRepository(QDbContext context,
                        IServiceScopeFactory serviceScopeFactory,
            IServiceProvider services,
            IBackgroundTaskQueue queue) : base(context)
        {
            _context = context;
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));

        }

        public async Task<List<Event>> GetEvents(List<string> clientsIds)
        {
            var events = await _context.Events.Where(
                    e => clientsIds.Contains(e.CreatedById) &&
                    e.Status == Notification.EventStatus.Unprocessed
                )
                .ToListAsync();


            QueueNoOfFails(clientsIds);

            return events;
        }

        public async Task<List<ClientsEvents>> GetClientEvents(List<string> clientsIds)
        {
            var events = await _context.Events.Where(
                    e => clientsIds.Contains(e.CreatedById) &&
                    e.Status == Notification.EventStatus.Unprocessed
                )
                //.Select(e => new
                //{
                //    CreatedById = e.CreatedById,
                //    Value = e.Value
                //})
                .ToListAsync();
            QueueNoOfFails(clientsIds);

            var clientsEvents = events.GroupBy(e => e.CreatedById)
                .Select(group => new ClientsEvents
                {
                    UserId = group.Key,
                    EventsValues = group.Select(e => JsonConvert.DeserializeObject<object>(e.Value)).ToList()
                })
                .ToList();

            return clientsEvents;
        }

        private void QueueNoOfFails(List<string> clientsIds)
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var scopedContext = scopedServices.GetRequiredService<IEventRepository>();

                    var scopedEvents = await scopedContext.Query(e => clientsIds.Contains(e.CreatedById) &&
                    e.Status == Notification.EventStatus.Unprocessed)
                    .ToListAsync();

                    foreach (var scopevent in scopedEvents)
                    {
                        var increament = scopevent.NoOfFails++;
                        scopevent.NoOfFails = scopevent.NoOfFails++;

                        if (increament > 2)
                        {
                            scopevent.Status = Notification.EventStatus.Archive;
                        }
                        await scopedContext.Update(scopevent, null, false);
                    }

                    await scopedContext.Save();
                }
            });
        }

        public async Task InsertProfileImageReadyEvent(Event userNotification, IdentityUser user)
        {
            var profileImageReadyEvent = await _context.Events.Where(
             e => !e.IsDeleted && e.CreatedById == user.Id
             && e.EventType == FileTypes.Images.ProfileImage
           )
             .ToListAsync();

            if (profileImageReadyEvent.Count() > 0)
            {
                foreach (var notif in profileImageReadyEvent)
                {

                    await base.Delete(notif.ID, null);
                }
                await base.Insert(userNotification, user);
                //await base.Save();
            }
            else
            {
                await base.Insert(userNotification, user);
            }
        }
    }
}
