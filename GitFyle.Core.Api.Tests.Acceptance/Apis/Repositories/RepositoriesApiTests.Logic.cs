// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
            DateTimeOffset currentDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(currentDate);
            Source inputSource = randomSource;

            Repository randomRepository = CreateRandomRepository(currentDate);
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
            DateTimeOffset currentDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(currentDate);
            Source inputSource = randomSource;

            Repository randomRepository = CreateRandomRepository(currentDate);
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
        public async Task ShouldGetAllRepositoriesAsync()
        {
            // given
            DateTimeOffset someDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(someDate);
            await this.gitFyleCoreApiBroker.PostSourceAsync(randomSource);

            List<Repository> inputRepositories =
                await GeneratePostedRepositoriesAsync(randomSource);
            IEnumerable<Repository> expectedRepositories = inputRepositories;

            // when
            IEnumerable<Repository> actualRepositories =
                await this.gitFyleCoreApiBroker.GetAllRepositoriesAsync();

            // then
            await RemovePostedRepositoriesAsync(expectedRepositories, actualRepositories);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(randomSource.Id);
        }

        [Fact]
        public async Task ShouldPutRepositoryAsync()
        {
            // given
            DateTimeOffset someDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(someDate);
            await this.gitFyleCoreApiBroker.PostSourceAsync(randomSource);
            Repository randomRepository = CreateRandomRepository(someDate);
            randomRepository.SourceId = randomSource.Id;
            await this.gitFyleCoreApiBroker.PostRepositoryAsync(randomRepository);
            Repository modifiedRepository = ModifyRandomRepository(randomRepository);

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
            DateTimeOffset someDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(someDate);
            await this.gitFyleCoreApiBroker.PostSourceAsync(randomSource);
            Repository randomRepository = CreateRandomRepository(someDate);
            randomRepository.SourceId = randomSource.Id;
            await this.gitFyleCoreApiBroker.PostRepositoryAsync(randomRepository);
            Repository modifiedRepository = ModifyRandomRepository(randomRepository);

            // when
            await this.gitFyleCoreApiBroker.PutRepositoryAsync(modifiedRepository);
            Repository actualRepository =
                await this.gitFyleCoreApiBroker.GetRepositoryByIdAsync(modifiedRepository.Id);

            // then
            actualRepository.Should().BeEquivalentTo(modifiedRepository);
            await this.gitFyleCoreApiBroker.DeleteRepositoryByIdAsync(actualRepository.Id);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(randomSource.Id);
        }
    }
}
