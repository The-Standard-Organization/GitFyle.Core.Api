// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions
{
    public class ContributorServiceException : Xeption
    {
        public ContributorServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}