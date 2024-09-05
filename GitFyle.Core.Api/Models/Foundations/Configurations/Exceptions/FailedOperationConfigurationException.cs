using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class FailedOperationConfigurationException : Xeption
    {
        public FailedOperationConfigurationException(string message, Exception innerException) : 
            base(message, innerException)
        { }
    }
}
