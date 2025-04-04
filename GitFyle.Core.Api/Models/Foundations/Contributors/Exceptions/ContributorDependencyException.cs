﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions
{
    public class ContributorDependencyException : Xeption
    {
        public ContributorDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}