using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Models.ReadModels
{
    public class NotificationViewModel
    {
        public string Id { get; set; }
        public string NotificationType { get; set; }

        public string Status { get; set; }

        public string CreatedDate { get { return CreatedDateD.ToString(DateFormat); } }

        public string DateFormat { get; set; }

        public DateTime CreatedDateD { get; set; }

        public object Value { get { return JsonConvert.DeserializeObject(ValueS); } }

        public string ValueS { get; set; }

        public int TotalCount { get; set; }

    }
}
