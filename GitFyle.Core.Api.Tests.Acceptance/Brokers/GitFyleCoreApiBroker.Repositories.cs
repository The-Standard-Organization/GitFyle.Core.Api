// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitFyle.Core.Api.Tests.Acceptance.Models.Repositories;

namespace GitFyle.Core.Api.Tests.Acceptance.Brokers
{
    public partial class GitFyleCoreApiBroker
    {
        private const string RepositoryRelativeUrl = "api/repositories";

        public async ValueTask<Repository> PostRepositoryAsync(Repository repository) =>
            await this.apiFactoryClient.PostContentAsync(RepositoryRelativeUrl, repository);

        public async ValueTask<Repository> GetRepositoryByIdAsync(Guid repositoryId) =>
            await this.apiFactoryClient.GetContentAsync<Repository>($"{RepositoryRelativeUrl}/{repositoryId}");

        public async ValueTask<IEnumerable<Repository>> GetAllRepositoriesAsync() =>
            await this.apiFactoryClient.GetContentAsync<IEnumerable<Repository>>(RepositoryRelativeUrl);

        public async ValueTask<Repository> PutRepositoryAsync(Repository repository) =>
            await this.apiFactoryClient.PutContentAsync(RepositoryRelativeUrl, repository);

        public async ValueTask<Repository> DeleteRepositoryByIdAsync(Guid repositoryId) =>
            await this.apiFactoryClient.DeleteContentAsync<Repository>($"{RepositoryRelativeUrl}/{repositoryId}");
    }
}
