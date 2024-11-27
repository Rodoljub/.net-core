using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Quantum.Utility.Infrastructure.Exceptions
{
    public class ExistingSaveSearchException : HttpStatusCodeException
    {
        public ExistingSaveSearchException(HttpStatusCode statusCode, string userMessage) : base(statusCode, userMessage)
        {
        }

        public ExistingSaveSearchException(HttpStatusCode statusCode, string userMessage, string localizationKey) : base(statusCode, userMessage, localizationKey)
        {
        }

        public ExistingSaveSearchException(HttpStatusCode statusCode, Exception innerException, string userMessage) : base(statusCode, innerException, userMessage)
        {
        }

        public ExistingSaveSearchException(HttpStatusCode statusCode, Exception innerException, string userMessage, string localizationKey) : base(statusCode, innerException, userMessage, localizationKey)
        {
        }

        public ExistingSaveSearchException(HttpStatusCode statusCode, string userMessage, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, innerException, logEntryPrefix)
        {
        }

        public ExistingSaveSearchException(HttpStatusCode statusCode, string userMessage, string localizationKey, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, localizationKey, innerException, logEntryPrefix)
        {
        }
    }
}
