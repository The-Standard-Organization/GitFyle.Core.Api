// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public class FailedOperationSourceException : Xeption
    {
        public FailedOperationSourceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}