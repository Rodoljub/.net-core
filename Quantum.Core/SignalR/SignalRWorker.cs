using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quantum.Core.SignalR.Hubs;
using Quantum.Data.Models;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quantum.Core.SignalR
{
    public class SignalRWorker : IHostedService
    {
        private Thread _loopThread;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private ILogger<SignalRWorker> _logger;
        private IConfiguration _config;
        private IHubContext<EventsHub> _hub;
        private IEventRepository _eventRepo;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IServiceProvider Services { get; }
        public IBackgroundTaskQueue Queue { get; }


        public SignalRWorker(
            ILogger<SignalRWorker> logger,
            IConfiguration config,
            IHubContext<EventsHub> hub,
            IServiceScopeFactory serviceScopeFactory,
            IServiceProvider services,
            IBackgroundTaskQueue queue
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _hub = hub;
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SignalRWorker....");

            _loopThread = new Thread(() =>
            {
                _logger.LogInformation("Starting _loopThread....");

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var threadSleep = _config.GetAsInteger($"Application:SignalR:ThreadSleepMs", 65000);
                    Thread.Sleep(threadSleep);
                    try
                    {
                        //await GetSendEvents();

                        QueueScopedEvents();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.StackTrace, ex.Message);
                        //throw;
                    }
                }
            });

            _loopThread.Start();
            return Task.CompletedTask;
        }

        private void QueueScopedEvents()
        {
            Queue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var scopedEvents = scopedServices.GetRequiredService<IEventRepository>();

                    //var scopedHub = scopedServices.GetRequiredService<Hub>();

                    var clientsIds = SignalRListIdsHandler.ConnectedIds;

                    _logger.LogInformation($"SignalR ClientIds Count: {clientsIds.Count()}");

                    if (clientsIds.Count() > 0)
                    {
                        var events = await scopedEvents.GetEvents(clientsIds);

                        _logger.LogInformation($"SignalR events count: {events.Count()}");
                        if (events.Count() > 0)
                        {
                            var clientsEvents =
                            events.GroupBy(e => e.CreatedById)
                            .Select(group => new ClientsEvents
                            {
                                UserId = group.Key,
                                EventsValues = group.Select(e => JsonConvert.DeserializeObject<object>(e.Value)).ToList()
                            })
                            .ToList();

                            //await scopedEvents.GetClientEvents(clientsIds);
                            _logger.LogInformation($"SignalR clientEvents count: {clientsEvents.Count()}");

                            foreach (var client in clientsEvents)
                            {

                                await _hub.Clients.Group(client.UserId).SendAsync("Events",
                                    JsonConvert.SerializeObject(client.EventsValues));

                                _logger.LogInformation($"SignalR SendAsync clientId: {client.UserId}");
                                var eve = events.Where(e => e.CreatedById == client.UserId).ToList();

                                foreach (var e in eve)
                                {
                                    e.Status = Notification.EventStatus.Processed;
                                    await scopedEvents.Update(e, null);
                                }

                                await scopedEvents.Save();



                                //await _hub.Clients.All.SendAsync("Send", "messssss");
                            }
                        }

                    }
                }
            });
        }

        //private async Task GetSendEvents()
        //{
        //    var clientsIds = SignalRListIdsHandler.ConnectedIds;

        //    if (clientsIds.Count() > 0)
        //    {
        //        var clientsEvents = await _eventRepo.GetClientEvents(clientsIds);

        //        foreach (var client in clientsEvents)
        //        {

        //            await _hub.Clients.Group(client.UserId).SendAsync("Events",
        //                JsonConvert.SerializeObject(client.EventsValues));


        //            //await _hub.Clients.All.SendAsync("Send", "messssss");
        //        }
        //    }
        //}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stoping _loopThread....");
            _cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }
    }
}
