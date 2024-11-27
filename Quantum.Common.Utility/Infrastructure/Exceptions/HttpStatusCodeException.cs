using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Quantum.Utility.Infrastructure.Exceptions
{
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCodeException(HttpStatusCode statusCode, string userMessage) 
            : base(userMessage)
        {
            this.StatusCode = statusCode;
            this.UserMessage = userMessage;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string userMessage, string localizationKey)
           : base(userMessage)
        {
            this.StatusCode = statusCode;
            this.UserMessage = userMessage;
            this.LocalizationKey = localizationKey;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, Exception innerException, string userMessage) 
            : base(userMessage, innerException)
        {
            this.StatusCode = statusCode;
            this.UserMessage = userMessage;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, Exception innerException, string userMessage, string localizationKey)
           : base(userMessage, innerException)
        {
            this.StatusCode = statusCode;
            this.UserMessage = userMessage;
            this.LocalizationKey = localizationKey;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string userMessage, Exception innerException, string logEntryPrefix)
          : base(userMessage, innerException)
        {
            this.StatusCode = statusCode;
            this.UserMessage = userMessage;
            this.LogEntryPrefix = logEntryPrefix;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string userMessage, string localizationKey, Exception innerException, string logEntryPrefix)
         : base(userMessage, innerException)
        {
            this.StatusCode = statusCode;
            this.UserMessage = userMessage;
            this.LogEntryPrefix = logEntryPrefix;
            this.LocalizationKey = localizationKey;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string UserMessage { get; set; }

        public string LocalizationKey { get; set; }

        public string LogEntryPrefix { get; set; }

    }
}
