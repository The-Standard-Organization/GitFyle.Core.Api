// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.ContributionTypes
{
    internal partial class ContributionTypeService
    {
        private void ValidateContributionTypeOnAddAsync(ContributionType contributionType)
        {
            ValidateContributionTypeIsNotNull(contributionType);
        }

        private static void ValidateContributionTypeIsNotNull(ContributionType contributionType)
        {
            if (contributionType is null)
            {
                throw new NullContributionTypeException(message: "ContributionType is null");
            }
        }
    }
}