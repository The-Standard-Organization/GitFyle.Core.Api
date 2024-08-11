// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    internal class FailedOperationSourceException : Xeption
    {
        private string message;
        private DbUpdateException innerException;

        public FailedOperationSourceException(string message, DbUpdateException innerException)
            : base(message, innerException)
        { }
    }
}