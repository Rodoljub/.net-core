using Microsoft.AspNetCore.Identity;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
    public interface INotificationService
    {
      
        Task ProjectFileReady(Item item, IdentityUser user);
        Task ProfileImageReady(File file, IdentityUser user);
        
        Task<List<NotificationViewModel>> GetNotifications(int skip, IIdentity identity);
        Task UpdateNotificationsViewed(IIdentity identity);

        Task DeleteNotification(string itemId, IIdentity identity);
        Task<List<NotificationViewModel>> GetNewNotifications(int totalCount, IIdentity identity);
    }
}
