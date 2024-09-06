// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class FailedServiceConfigurationException : Xeption
    {
        public FailedServiceConfigurationException(string message, Exception innerException) 
            : base(message, innerException)
        { }
    }
}
