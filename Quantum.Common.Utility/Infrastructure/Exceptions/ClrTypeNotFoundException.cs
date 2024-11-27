using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Quantum.Utility.Infrastructure.Exceptions
{
	public class ClrTypeNotFoundException : HttpStatusCodeException
	{
		public ClrTypeNotFoundException(HttpStatusCode statusCode, string userMessage) : base(statusCode, userMessage)
		{
		}

		public ClrTypeNotFoundException(HttpStatusCode statusCode, string userMessage, string localizationKey) : base(statusCode, userMessage, localizationKey)
		{
		}

		public ClrTypeNotFoundException(HttpStatusCode statusCode, Exception innerException, string userMessage) : base(statusCode, innerException, userMessage)
		{
		}

		public ClrTypeNotFoundException(HttpStatusCode statusCode, Exception innerException, string userMessage, string localizationKey) : base(statusCode, innerException, userMessage, localizationKey)
		{
		}

		public ClrTypeNotFoundException(HttpStatusCode statusCode, string userMessage, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, innerException, logEntryPrefix)
		{
		}

		public ClrTypeNotFoundException(HttpStatusCode statusCode, string userMessage, string localizationKey, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, localizationKey, innerException, logEntryPrefix)
		{
		}
	}
}
