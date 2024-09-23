// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions
{
    public class RepositoryDependencyException : Xeption
    {
        public RepositoryDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}