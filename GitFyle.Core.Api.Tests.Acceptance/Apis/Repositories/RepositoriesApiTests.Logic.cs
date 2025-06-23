// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
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
            Source randomSource = CreateRandomSource();
            Source inputSource = randomSource;
            Repository randomRepository = CreateRandomRepository(inputSource.Id);
            Repository inputRepository = randomRepository;
            Repository expectedRepository = inputRepository.DeepClone();

            // when
            await this.gitFyleCoreApiBroker.PostSourceAsync(inputSource);
            inputRepository.SourceId = inputSource.Id;
            expectedRepository.SourceId = inputSource.Id;
            await this.gitFyleCoreApiBroker.PostRepositoryAsync(inputRepository);

            Repository actualRepository =
                await this.gitFyleCoreApiBroker.GetRepositoryByIdAsync(inputRepository.Id);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);
            await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(actualRepository.Id);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(inputSource.Id);
        }

        [Fact]
        public async Task ShouldGetRepositoryByIdAsync()
        {
            // given
            Source randomSource = CreateRandomSource();
            Source inputSource = randomSource;
            Repository randomRepository = CreateRandomRepository(inputSource.Id);
            Repository inputRepository = randomRepository;
            Repository expectedRepository = inputRepository.DeepClone();

            // when
            await this.gitFyleCoreApiBroker.PostSourceAsync(inputSource);
            randomRepository.SourceId = inputSource.Id;
            expectedRepository.SourceId = inputSource.Id;
            await this.gitFyleCoreApiBroker.PostRepositoryAsync(randomRepository);

            Repository actualRepository =
                await this.gitFyleCoreApiBroker.GetRepositoryByIdAsync(randomRepository.Id);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);
            await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(actualRepository.Id);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(inputSource.Id);
        }

        [Fact]
        public async Task ShouldGetAllRepositoriesAsync()
        {
            // given
            Source randomSource = CreateRandomSource();

            List<Repository> randomRepositories =
                CreateRandomRepositories(randomSource.Id);

            List<Repository> expectedRepositories =
                randomRepositories;

            // when
            List<Repository> actualRepositories =
                await this.gitFyleCoreApiBroker.GetAllRepositoriesAsync();

            // then
            foreach (Repository expectedRepository in actualRepositories)
            {
                Repository actualRepository =
                    actualRepositories.Single(
                        product => product.Id == expectedRepository.Id);

                actualRepository.Should().BeEquivalentTo(expectedRepository);

                await this.gitFyleCoreApiBroker
                    .DeleteRepositoryByIdAsync(actualRepository.Id);
            }
        }

        [Fact]
        public async Task ShouldPutRepositoryAsync()
        {
            // given
            Source randomSource = await PostRandomSourceAsync();

            Repository modifiedRepository =
                await ModifyRandomRepository(sourceId: randomSource.Id);

            // when
            await this.gitFyleCoreApiBroker.PutRepositoryAsync(modifiedRepository);

            Repository actualRepository =
                await this.gitFyleCoreApiBroker.GetRepositoryByIdAsync(modifiedRepository.Id);

            // then
            actualRepository.Should().BeEquivalentTo(modifiedRepository);
            await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(actualRepository.Id);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(randomSource.Id);
        }

        [Fact]
        public async Task ShouldDeleteRepositoryAsync()
        {
            // given
            Source randomSource = await PostRandomSourceAsync();
            Repository randomRepository = await PostRandomRepository(sourceId: randomSource.Id);
            Repository inputRepository = randomRepository;
            Repository expectedRepository = inputRepository.DeepClone();

            // when
            Repository deleteRepository =
                await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(inputRepository.Id);

            // then
            deleteRepository.Should().BeEquivalentTo(expectedRepository);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(randomSource.Id);
        }
    }
}
