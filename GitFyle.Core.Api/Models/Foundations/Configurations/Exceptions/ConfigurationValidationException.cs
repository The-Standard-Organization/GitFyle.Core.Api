// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class ConfigurationValidationException : Xeption
    {
        public ConfigurationValidationException(string message, Xeption innerException) 
            : base(message, innerException) 
        { }
    }
}
