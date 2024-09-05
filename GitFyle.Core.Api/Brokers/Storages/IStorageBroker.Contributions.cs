// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<Contribution> InsertContributionAsync(Contribution contribution);
        ValueTask<IQueryable<Contribution>> SelectAllContributionsAsync();
        ValueTask<Contribution> SelectContributionByIdAsync(Guid contributionId);
        ValueTask<Contribution> UpdateContributionAsync(Contribution contribution);
        ValueTask<Contribution> DeleteContributionAsync(Contribution contribution);
    }
}
