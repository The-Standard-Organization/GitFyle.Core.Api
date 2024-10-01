// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;

namespace GitFyle.Core.Api.Services.Foundations.Contributions
{
    public interface IContributionService
    {
        ValueTask<Contribution> AddContributionAsync(Contribution contribution);
        ValueTask<IQueryable<Contribution>> RetrieveAllContributionsAsync();
        ValueTask<Contribution> RetrieveContributionByIdAsync(Guid contributionId);
    }
}
