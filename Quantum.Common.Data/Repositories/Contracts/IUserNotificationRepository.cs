using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using Quantum.Data.Repositories.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IUserNotificationRepository : IBaseRepository<UserNotification, IdentityUser>
    {
        Task<List<NotificationViewModel>> GetNotifications(int skip, int take, IdentityUser user, int itemsCount);
        Task UpdateNotificationsViewed(IdentityUser user);
        Task<int> CountNotificationsByUserId(string id);
        Task InsertProfileImageReady(UserNotification userNotification, IdentityUser user);
    }
}
