using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Sources;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker   
    {
        ValueTask<Contributor> InsertContributorAsync(Contributor contributor);
    }
}
