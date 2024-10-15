// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions
{
    public class FailedStorageContributionTypeException : Xeption
    {
        public FailedStorageContributionTypeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}