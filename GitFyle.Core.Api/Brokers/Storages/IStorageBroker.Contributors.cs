using GitFyle.Core.Api.Models.Foundations.Contributors;
using System;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker   
    {
        public ValueTask<Contributor> SelectContributorByIdAsync(Guid contributorId);
    }
}
