// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions
{
    public class LockedConfigurationException : Xeption
    {
        public LockedConfigurationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
