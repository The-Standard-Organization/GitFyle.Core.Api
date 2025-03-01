using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Tests.Acceptance.Models.Repositories;
using GitFyle.Core.Api.Tests.Acceptance.Models.Sources;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Repositories
{
    public partial class RepositoriesApiTests
    {
        [Fact]
        public async Task ShouldPostRepositoryAsync()
        {
            // given
            DateTimeOffset currentDate = DateTimeOffset.UtcNow;
            Source someSource = await PostRandomSourceAsync();
            Repository randomRepository = CreateRandomRepository(currentDate);
            Repository inputRepository = randomRepository;
            inputRepository.SourceId = someSource.Id;
            Repository expectedRepository = inputRepository;

            // when
            Repository actualRepository =
                await this.gitFyleCoreApiBroker.PostRepositoryAsync(inputRepository);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);
            await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(actualRepository.Id);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(someSource.Id);
        }
    }
}
