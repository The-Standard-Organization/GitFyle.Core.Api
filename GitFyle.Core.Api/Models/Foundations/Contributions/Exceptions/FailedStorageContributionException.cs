// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions
{
    public class FailedStorageContributionException : Xeption
    {
        public FailedStorageContributionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
