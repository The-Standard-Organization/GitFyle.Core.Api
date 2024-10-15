// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions
{
    public class ContributorDependencyValidationException : Xeption
    {
        public ContributorDependencyValidationException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}