// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    internal class FailedOperationContributionException : Xeption
    {
        public FailedOperationContributionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}