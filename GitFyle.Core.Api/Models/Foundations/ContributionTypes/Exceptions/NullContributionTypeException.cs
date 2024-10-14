// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions
{
    public class NullContributionTypeException : Xeption
    {
        public NullContributionTypeException(string message)
            : base(message)
        { }

    }
}