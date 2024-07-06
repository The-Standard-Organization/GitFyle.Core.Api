// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Linq;
using GitFyle.Core.Api.Models.Foundations.Contributions;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<Contribution> InsertContributionAsync(Contribution contribution);
        IQueryable<Contribution> SelectAllContributionsAsync();
        ValueTask<Contribution> SelectContributionByIdAsync(Guid contributionId);
    }
}
