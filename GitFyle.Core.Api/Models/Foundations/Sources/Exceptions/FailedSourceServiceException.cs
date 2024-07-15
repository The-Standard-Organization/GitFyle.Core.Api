using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Sources.Exceptions
{
    public class FailedSourceServiceException : Xeption
    {
        public FailedSourceServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}