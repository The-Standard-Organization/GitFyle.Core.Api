using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Sources.Exceptions
{
    public class SourceServiceException : Xeption
    {
        public SourceServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}