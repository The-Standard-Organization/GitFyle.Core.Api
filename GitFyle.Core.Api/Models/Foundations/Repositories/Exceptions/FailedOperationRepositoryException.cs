// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions
{
    public class FailedOperationRepositoryException : Xeption
    {
        public FailedOperationRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}