using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Sources.Exceptions
{
    public class FailedSourceStorageException : Xeption
    {
        public FailedSourceStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}