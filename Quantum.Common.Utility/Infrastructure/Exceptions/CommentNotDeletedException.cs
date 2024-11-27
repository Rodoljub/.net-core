using System;
using System.Net;
using System.Runtime.Serialization;

namespace Quantum.Utility.Infrastructure.Exceptions
{
	public class CommentNotDeletedException : HttpStatusCodeException
	{
		public CommentNotDeletedException(HttpStatusCode statusCode, string userMessage) : base(statusCode, userMessage)
		{
		}

		public CommentNotDeletedException(HttpStatusCode statusCode, string userMessage, string localizationKey) : base(statusCode, userMessage, localizationKey)
		{
		}

		public CommentNotDeletedException(HttpStatusCode statusCode, Exception innerException, string userMessage) : base(statusCode, innerException, userMessage)
		{
		}

		public CommentNotDeletedException(HttpStatusCode statusCode, Exception innerException, string userMessage, string localizationKey) : base(statusCode, innerException, userMessage, localizationKey)
		{
		}

		public CommentNotDeletedException(HttpStatusCode statusCode, string userMessage, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, innerException, logEntryPrefix)
		{
		}

		public CommentNotDeletedException(HttpStatusCode statusCode, string userMessage, string localizationKey, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, localizationKey, innerException, logEntryPrefix)
		{
		}
	}
}
