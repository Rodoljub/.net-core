using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
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
    public class UserNotificationRepository : BaseRepository<UserNotification>, IUserNotificationRepository
    {
		private QDbContext _context;
        private IUtilityService _utilServ;
        public UserNotificationRepository(
			QDbContext context,
            IUtilityService utilServ
        ) : base(context)
		{
			_context = context;
            _utilServ = utilServ;
		}

        public async Task<int> CountNotificationsByUserId(string id)
        {
            var notifView = await _context.UserNotifications
                   .Where(n => n.CreatedById == id && !n.IsDeleted && !n.Item.IsDeleted && !n.Item.File.IsDeleted).CountAsync();

            return notifView;
        }


        public async Task<List<NotificationViewModel>> GetNotifications(int skip, int take, IdentityUser user, int itemsCount)
        {
            var userId = string.Empty;
            if (user != null)
            {
                userId = user.Id;
            }

            var dateFormat = "MM/dd/yyyy HH:mm:ss";

            var notifView = await _context.UserNotifications
                    .Where(n => n.CreatedById == userId && !n.IsDeleted && !n.Item.IsDeleted && !n.Item.File.IsDeleted)// && n.Status == Notification.Status.Unviewed)
                    .AsNoTracking()
                    .OrderByDescending(n => n.CreatedDate)
                    .Select(n => new NotificationViewModel()
                    {
                        Id = n.ID,
                        NotificationType = n.NotificationType,
                        Status = n.Status,
                        //CreatedDate = _utilServ.TimeAgo(n.CreatedDate),
                        //CreatedDate = n.CreatedDate.ToString("MM/dd/yyyy HH:mm:ss"),
                        CreatedDateD = n.CreatedDate,
                        DateFormat = dateFormat,
                        //Value = JsonConvert.DeserializeObject(n.Value.ToString()),
                        ValueS = n.Value,
                        TotalCount = itemsCount
                    })
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

            return notifView;
        }

        public async Task InsertProfileImageReady(UserNotification userNotification, IdentityUser user)
        {
            var profileImageReadyNotifs = await _context.UserNotifications.Where(
             n => !n.IsDeleted && n.CreatedById == user.Id
             && n.NotificationType == FileTypes.Images.ProfileImage
           )
             .ToListAsync();

            if (profileImageReadyNotifs.Count() > 0)
            {
                foreach (var notif in profileImageReadyNotifs)
                {

                    await base.Delete(notif.ID, null);
                }
                await base.Insert(userNotification, user);
                //await base.Save();
            } else
            {
                await base.Insert(userNotification, user);
            }
        }

        public async Task UpdateNotificationsViewed(IdentityUser user)
        {
            var notificationsUnviewed = await _context.UserNotifications.Where(
                    n => n.CreatedById == user.Id &&
                    n.Status == Notification.Status.Unviewed
                )
                .ToListAsync();

            foreach(var notification in notificationsUnviewed)
            {
                notification.Status = Notification.Status.Viewed;

                await base.Update(notification, user, false);
            }

            await base.Save();
        }
    }
}
