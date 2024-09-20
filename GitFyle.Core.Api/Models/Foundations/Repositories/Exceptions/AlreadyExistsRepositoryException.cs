// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions
{
    public class AlreadyExistsRepositoryException : Xeption
    {
        public AlreadyExistsRepositoryException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}