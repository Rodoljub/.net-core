using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Utility.Dictionary
{
    public sealed class Notification
    {
        public sealed class Status
        {
            public const string Unviewed = "Unviewed";
            public const string Viewed = "Viewed";
            public const string Read = "Read";
        }

        public sealed class EventStatus
        {
            public const string Unprocessed = "Unprocessed";
            public const string Processed = "Processed";
            public const string Archive = "Archive";
        }

        public sealed class Messages
        {
            public const string ImageIsReady = "Image is ready";
            public const string ProfileImageReady = "Profile image is ready";
        }
    }
}
