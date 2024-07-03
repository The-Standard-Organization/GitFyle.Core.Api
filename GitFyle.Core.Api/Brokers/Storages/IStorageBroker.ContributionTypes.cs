// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        IQueryable<ContributionType> SelectAllContributionTypesAsync();
    }
}
