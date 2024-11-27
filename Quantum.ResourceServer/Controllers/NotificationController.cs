using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.ResourceServer.Controllers
{
    [EnableCors("GeneralPolicy")]
    [Produces("application/json")]
    [Route("api/notifications")]
    [ApiController]
    [Authorize(LocalApi.PolicyName)]
    public class NotificationController : Controller
    {
        private UserManager<IdentityUser> _userMgr;

        private INotificationService _notificationService;


        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IServiceProvider Services { get; }
        public IBackgroundTaskQueue Queue { get; }

        public NotificationController(
            UserManager<IdentityUser> userMgr,
      
            INotificationService notificationService,
   
                        IServiceScopeFactory serviceScopeFactory,
            IServiceProvider services,
            IBackgroundTaskQueue queue
        )
        {
            _userMgr = userMgr;
  
            _notificationService = notificationService;
      
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        [HttpGet("")]
        public async Task<IActionResult> GetNotifications([FromQuery] int skip)
        {
            var notifications = await _notificationService.GetNotifications(skip, User.Identity);
            //var value = JsonConvert.DeserializeObject(notification.Value.ToString());

            return Ok(notifications);
        }

        [HttpGet("new")]
        public async Task<IActionResult> GetNewNotifications([FromQuery] int totalCount)
        {
            var notifications = await _notificationService.GetNewNotifications(totalCount, User.Identity);
            //var value = JsonConvert.DeserializeObject(notification.Value.ToString());

            return Ok(notifications);
        }


        [HttpGet("update")]
        public async Task<IActionResult> UpdateNotifications()
        {
            await _notificationService.UpdateNotificationsViewed(User.Identity);

            return Ok();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteNotification([FromForm] string id)
        {
            await _notificationService.DeleteNotification(id, User.Identity);
            return Ok();
        }

        //[HttpPost("")]
        //public async Task<IActionResult> CreateProjectFileReady()
        //{
        //    var user = await _userMgr.FindByIdAsync("24f05a56-40bb-4c36-92ec-ec5b8022b956");
        //    var item = await _itemRepo.GetById("09f6e30ab7b948a6a6fb3a02f907f54b");

        //    QueueSetNotification(item, user);

        //    return Ok();
        //}

        //private void QueueSetNotification(Item item, IdentityUser user)
        //{
        //    Queue.QueueBackgroundWorkItem(async token =>
        //    {
        //        using (var scope = _serviceScopeFactory.CreateScope())
        //        {
        //            var scopedServices = scope.ServiceProvider;

        //            var scopedNotificationService = scopedServices.GetRequiredService<INotificationService>();

        //            await scopedNotificationService.ProjectFileReady(item, user);
        //        }
        //    });
        //}
    }
}
