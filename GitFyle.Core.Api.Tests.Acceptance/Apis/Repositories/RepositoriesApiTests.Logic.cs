// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
            DateTimeOffset currentPostDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(currentPostDate);
            Source inputSource = randomSource;

            Repository randomRepository = CreateRandomRepository(currentPostDate);
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
            DateTimeOffset currentPostDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(currentPostDate);
            Source inputSource = randomSource;

            Repository randomRepository = CreateRandomRepository(currentPostDate);
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
    }
}
