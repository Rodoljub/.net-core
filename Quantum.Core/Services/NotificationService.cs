using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
    public class NotificationService : INotificationService
    {
        private IConfiguration _config;
        private IUserManagerService _userMgrServ;
        private IUserNotificationRepository _userNotifRepo;
        private IEventRepository _eventRepo;

        public NotificationService(
            IConfiguration config,
            IUserManagerService userMgrServ,
            IUserNotificationRepository userNotifRepo,
            IEventRepository eventRepo
        )
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userMgrServ = userMgrServ ?? throw new ArgumentNullException(nameof(userMgrServ));
            _userNotifRepo = userNotifRepo ?? throw new ArgumentNullException(nameof(userNotifRepo));
            _eventRepo = eventRepo ?? throw new ArgumentNullException(nameof(eventRepo));
        }

        public async Task<List<NotificationViewModel>> GetNewNotifications(int totalCount, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);

            var itemsCount = await _userNotifRepo.CountNotificationsByUserId(user.Id);

            var take = itemsCount - totalCount;
            if (take > 0)
            {
                return await _userNotifRepo.GetNotifications(0, take, user, itemsCount);
            } else
            {
                return new List<NotificationViewModel>();
            }

        }

        public async Task<List<NotificationViewModel>> GetNotifications(int skip, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);
            var take = _config.GetAsInteger("Application:NotificationPageSize", 9);
            var itemsCount = await _userNotifRepo.CountNotificationsByUserId(user.Id);

            var notifViewModel = await _userNotifRepo.GetNotifications(skip, take, user, itemsCount);

            return notifViewModel;
        }

        public async Task DeleteNotification(string itemId, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);

            var notification = await _userNotifRepo.Query(n => !n.IsDeleted && n.ID == itemId
            && n.CreatedById == user.Id)
                .FirstOrDefaultAsync();

            if (notification != null)
            {
                await _userNotifRepo.Delete(notification.ID, user);
            }
        }

        public async Task ProjectFileReady(Item item, IdentityUser user)
        {
            var projectFileNotification = new
            {
                SubjectImage = item.FileID,
                SubjectUrl = "",
                Subject = "",
                Message = Notification.Messages.ImageIsReady,
                Url = item.ID
            };

            var userNotification = new UserNotification
            {
                CreatedById = user.Id,
                NotificationType = FileTypes.Images.ProjectFile,
                NotificationId = item.ID,
                ItemID = item.ID,
                Status = Notification.Status.Unviewed,
                Value = JsonConvert.SerializeObject(projectFileNotification)
            };

            Event eventNotification = new Event
            {
                EventType = FileTypes.Images.ProjectFile,
                EntityId = item.ID,
                Status = Notification.EventStatus.Unprocessed,
                Value = JsonConvert.SerializeObject(projectFileNotification)
            };

            await _eventRepo.Insert(eventNotification, user);

            await _userNotifRepo.Insert(userNotification, user);
        }

        public async Task ProfileImageReady(File file, IdentityUser user)
        {
            var projectFileNotification = new ProjectFileNotificationModel
            {
                SubjectImage = file.ID,
                SubjectUrl = "",
                Subject = "",
                Message = Notification.Messages.ProfileImageReady,
                Url = "profile"
            };

            var userNotification = new UserNotification
            {
                CreatedById = user.Id,
                NotificationType = FileTypes.Images.ProfileImage,
                NotificationId = file.ID,
                Status = Notification.Status.Unviewed,
                Value = JsonConvert.SerializeObject(projectFileNotification)
            };

            Event eventNotification = new Event
            {
                EventType = FileTypes.Images.ProfileImage,
                EntityId = file.ID,
                Status = Notification.EventStatus.Unprocessed,
                Value = JsonConvert.SerializeObject(projectFileNotification)
            };

            await _eventRepo.InsertProfileImageReadyEvent(eventNotification, user);

            await _userNotifRepo.InsertProfileImageReady(userNotification, user);
        }


        public async Task UpdateNotificationsViewed(IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);

            await _userNotifRepo.UpdateNotificationsViewed(user);
        }

    
    }
}
