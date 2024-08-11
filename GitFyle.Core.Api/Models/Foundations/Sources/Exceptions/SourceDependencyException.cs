// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Sources.Exceptions
{
    public class SourceDependencyException : Xeption
    {
        public SourceDependencyException(string message, Xeption innerException)
            : base(message, innerException) { }
    }
}
