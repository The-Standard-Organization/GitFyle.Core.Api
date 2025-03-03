using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Tests.Acceptance.Models.Repositories;

namespace GitFyle.Core.Api.Tests.Acceptance.Brokers
{
    public partial class GitFyleCoreApiBroker
    {
        private const string RepositoryRelativeUrl = "api/repositories";

        public async ValueTask<Repository> PostRepositoryAsync(Repository repository) =>
            await this.apiFactoryClient.PostContentAsync(RepositoryRelativeUrl, repository);

        public async ValueTask<Repository> DeleteRepositoryByIdAsync(Guid repositoryId) =>
            await this.apiFactoryClient.DeleteContentAsync<Repository>($"{RepositoryRelativeUrl}/{repositoryId}");
    }
}
