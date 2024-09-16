// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class InvalidConfigurationException : Xeption
    {
        public InvalidConfigurationException(string message)
            : base(message)
        { }
    }
}
