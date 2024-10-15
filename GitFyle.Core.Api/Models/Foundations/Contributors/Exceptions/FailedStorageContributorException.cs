// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions
{
    public class FailedStorageContributorException : Xeption
    {
        public FailedStorageContributorException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}