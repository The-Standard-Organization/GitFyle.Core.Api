using GitFyle.Core.Api.Models.Foundations.Contributors;
using System.Linq;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker   
    {
        IQueryable<Contributor> SelectAllContributorsAsync();
    }
}
