// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions
{
    public class InvalidRepositoryException : Xeption
    {
        public InvalidRepositoryException(string message)
            : base(message)
        { }
    }
}