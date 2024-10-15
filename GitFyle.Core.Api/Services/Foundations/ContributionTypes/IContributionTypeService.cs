// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;

namespace GitFyle.Core.Api.Services.Foundations.ContributionTypes
{
    public interface IContributionTypeService
    {
        ValueTask<ContributionType> AddContributionTypeAsync(ContributionType contributionType);
        ValueTask<IQueryable<ContributionType>> RetrieveAllContributionTypesAsync();
        ValueTask<ContributionType> RetrieveContributionTypeByIdAsync(Guid contributionTypeId);
        ValueTask<ContributionType> ModifyContributionTypeAsync(ContributionType contributionType);
        ValueTask<ContributionType> RemoveContributionTypeByIdAsync(Guid contributionTypeId);
    }
}