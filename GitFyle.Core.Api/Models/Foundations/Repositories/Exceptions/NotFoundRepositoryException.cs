// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions
{
    public class NotFoundRepositoryException : Xeption
    {
        public NotFoundRepositoryException(string message)
            : base(message)
        { }
    }
}