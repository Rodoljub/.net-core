using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Quantum.Utility.Infrastructure.Exceptions
{
    public class GeneralErrorException : HttpStatusCodeException
    {
        public GeneralErrorException(HttpStatusCode statusCode, string userMessage) : base(statusCode, userMessage)
        {
        }

        public GeneralErrorException(HttpStatusCode statusCode, string userMessage, string localizationKey) : base(statusCode, userMessage, localizationKey)
        {
        }

        public GeneralErrorException(HttpStatusCode statusCode, Exception innerException, string userMessage) : base(statusCode, innerException, userMessage)
        {
        }

        public GeneralErrorException(HttpStatusCode statusCode, Exception innerException, string userMessage, string localizationKey) : base(statusCode, innerException, userMessage, localizationKey)
        {
        }

        public GeneralErrorException(HttpStatusCode statusCode, string userMessage, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, innerException, logEntryPrefix)
        {
        }

        public GeneralErrorException(HttpStatusCode statusCode, string userMessage, string localizationKey, Exception innerException, string logEntryPrefix) : base(statusCode, userMessage, localizationKey, innerException, logEntryPrefix)
        {
        }
    }
}
