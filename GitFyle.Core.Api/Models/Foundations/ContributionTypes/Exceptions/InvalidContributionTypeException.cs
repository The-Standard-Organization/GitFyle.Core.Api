// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions
{
    public class InvalidContributionTypeException : Xeption
    {
        public InvalidContributionTypeException(string message)
            : base(message)
        { }
    }
}