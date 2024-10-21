// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions
{
    public class NotFoundContributionTypeException : Xeption
    {
        public NotFoundContributionTypeException(string message)
            : base(message)
        { }
    }
}