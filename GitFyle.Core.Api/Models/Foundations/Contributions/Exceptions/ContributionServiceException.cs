// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions
{
    public class ContributionServiceException : Xeption
    {
        public ContributionServiceException(string message, Xeption innerException)
            : base(message, innerException) { }
    }
}
