using GitFyle.Core.Api.Models.Foundations.Contributors;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker   
    {
        public ValueTask<Contributor> DeleteContributorAsync(Contributor contributor);
    }
}
