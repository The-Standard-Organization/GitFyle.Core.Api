﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Sources.Exceptions
{
    public class SourceValidationException : Xeption
    {
        public SourceValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
