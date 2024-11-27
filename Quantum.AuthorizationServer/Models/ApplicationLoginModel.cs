using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Models
{
    public class ApplicationLoginModel
    {
		public string AppName { get; set; }

		public string Secret { get; set; }
	}
}
