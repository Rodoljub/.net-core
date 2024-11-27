using Quantum.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Models
{
    public class ClientsEvents
    {
        public string UserId { get; set; }

        public List<object> EventsValues { get; set; }
    }
}
