using GitFyle.Core.Api.Models.Foundations.Repositories;
using System.Linq;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        IQueryable<Repository> SelectAllRepositoriesAsync();
    }
}
