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
            Source randomSource = await PostRandomSourceAsync();
            Repository randomRepository = await PostRandomRepository(sourceId: randomSource.Id);
            Repository inputRepository = randomRepository;
            Repository expectedRepository = inputRepository.DeepClone();

            // when
            Repository actualRepository =
                await this.gitFyleCoreApiBroker.GetRepositoryByIdAsync(inputRepository.Id);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);
            await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(actualRepository.Id);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(randomSource.Id);
        }

        [Fact]
        public async Task ShouldGetRepositoryByIdAsync()
        {
            // given
            Source randomSource = await PostRandomSourceAsync();
            Repository randomRepository = await PostRandomRepository(sourceId: randomSource.Id);
            Repository inputRepository = randomRepository;
            Repository expectedRepository = inputRepository.DeepClone();

            // when
            Repository actualRepository =
                await this.gitFyleCoreApiBroker.GetRepositoryByIdAsync(inputRepository.Id);

            // then
            actualRepository.Should().BeEquivalentTo(expectedRepository);
            await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(actualRepository.Id);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(randomSource.Id);
        }

        [Fact]
        public async Task ShouldGetAllRepositoriesAsync()
        {
            // given
            Source randomSource = await PostRandomSourceAsync();

            List<Repository> inputRepositories =
                await PostRandomRepositoriesAsync(sourceId: randomSource.Id);

            IEnumerable<Repository> expectedRepositories = inputRepositories;

            // when
            IEnumerable<Repository> actualRepositories =
                await this.gitFyleCoreApiBroker.GetAllRepositoriesAsync();

            // then
            foreach (Repository expectedRepository in expectedRepositories)
            {
                Repository actualRepository = 
                    actualRepositories.Single(repository => repository.Id == expectedRepository.Id);

                actualRepository.Should().BeEquivalentTo(expectedRepository);
                await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(actualRepository.Id);
            }
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(randomSource.Id);
        }

        [Fact]
        public async Task ShouldPutRepositoryAsync()
        {
            // given
            Source randomSource = await PostRandomSourceAsync();
            Repository modifiedRepository = await ModifyRandomRepository(sourceId: randomSource.Id);

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
