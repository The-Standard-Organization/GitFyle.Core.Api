// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions
{
    public class NotFoundContributorException : Xeption
    {
        public NotFoundContributorException(string message)
            : base(message)
        { }
    }
}