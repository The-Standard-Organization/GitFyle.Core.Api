// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions
{
    public class FailedServiceContributionException : Xeption
    {
        public FailedServiceContributionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
