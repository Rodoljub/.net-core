using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Quantum.Utility.Infrastructure.Exceptions
{
	public class CommentsNotFoundException : HttpStatusCodeException
	{
		public CommentsNotFoundException(HttpStatusCode statusCode, string userMessage) : base(statusCode, userMessage)
		{
		}

		public CommentsNotFoundException(HttpStatusCode statusCode, string userMessage, string localizationKey) : base(statusCode, userMessage, localizationKey)
		{
		}

		public CommentsNotFoundException(HttpStatusCode statusCode, Exception innerException, string userMessage) : base(statusCode, innerException, userMessage)
		{
		}

		public CommentsNotFoundException(HttpStatusCode statusCode, Exception innerException, string userMessage, string localizationKey) : base(statusCode, innerException, userMessage, localizationKey)
		{
		}

		public CommentsNotFoundException(HttpStatusCode statusCode, string userMessage, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, innerException, logEntryPrefix)
		{
		}

		public CommentsNotFoundException(HttpStatusCode statusCode, string userMessage, string localizationKey, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, localizationKey, innerException, logEntryPrefix)
		{
		}
	}
}
