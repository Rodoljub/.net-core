using Quantum.Utility.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Models
{
    internal class ProjectFileNotificationModel
    {
        public string SubjectImage { get; set; }

        public string SubjectUrl { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = Notification.Messages.ProfileImageReady;

        public string Url { get; set; } = "profile";

    }
}
