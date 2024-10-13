// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;

namespace GitFyle.Core.Api.Services.Foundations.ContributionTypes
{
    public interface IContributionTypeService
    {
        ValueTask<ContributionType> AddContributionTypeAsync(ContributionType repository);
    }
}