// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributors;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker   
    {
        public ValueTask<Contributor> SelectContributorByIdAsync(Guid contributorId);
        public ValueTask<Contributor> UpdateContributorAsync(Contributor contributor);
        public ValueTask<Contributor> DeleteContributorAsync(Contributor contributor);
    }
}
