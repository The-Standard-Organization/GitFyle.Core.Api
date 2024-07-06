// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<ContributionType> InsertContributionTypeAsync(ContributionType contributionType);
        ValueTask<ContributionType> UpdateContributionTypeAsync(ContributionType contributionType);
    }
}
