// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions
{
    public class InvalidContributorException : Xeption
    {
        public InvalidContributorException(string message)
            : base(message)
        { }
    }
}