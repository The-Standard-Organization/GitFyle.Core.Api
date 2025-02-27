// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Tests.Acceptance.Brokers;
using GitFyle.Core.Api.Tests.Acceptance.Models.Sources;
using RESTFulSense.Exceptions;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Sources
{
    [Collection(nameof(ApiTestCollection))]
    public partial class SourcesApiTests
    {
        [Fact]
        public async Task ShouldPostSourceAsync()
        {
            // given
            DateTimeOffset currentDate = DateTimeOffset.UtcNow;
            Source randomSource = CreateRandomSource(currentDate);
            Source inputSource = randomSource;
            Source expectedSource = inputSource;

            // when
            Source actualSource =
                await this.gitFyleCoreApiBroker.PostSourceAsync(inputSource);

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(actualSource.Id);
        }

        [Fact]
        public async Task ShouldGetAllSourcesAsync()
        {
            // given
            IEnumerable<Source> randomSources = CreateRandomSources();
            IEnumerable<Source> inputSources = randomSources;

            foreach (Source source in inputSources)
            {
                await this.gitFyleCoreApiBroker.PostSourceAsync(source);
            }

            IEnumerable<Source> expectedSources = inputSources;

            // when
            IEnumerable<Source> actualSources = await this.gitFyleCoreApiBroker.GetAllSourcesAsync();

            // then
            foreach (Source expectedSource in expectedSources)
            {
                Source actualSource = actualSources.Single(source => source.Id == expectedSource.Id);
                actualSource.Should().BeEquivalentTo(expectedSource);
                await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(actualSource.Id);
            }
        }

        [Fact]
        public async Task ShouldPutSourceAsync()
        {
            // given
            Source randomSource = await PostRandomSourceAsync();
            Source modifiedSource = UpdateRandomSource(randomSource);

            // when
            await this.gitFyleCoreApiBroker.PutSourceAsync(modifiedSource);

            Source actualSource =
                await this.gitFyleCoreApiBroker.GetSourceByIdAsync(randomSource.Id);

            // then
            actualSource.Should().BeEquivalentTo(modifiedSource);
            await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(actualSource.Id);
        }

        [Fact]
        public async Task ShouldDeleteSourceAsync()
        {
            // given
            Source randomSource = await PostRandomSourceAsync();
            Source inputSource = randomSource;
            Source expectedSource = inputSource;

            // when 
            Source deletedSource =
                await this.gitFyleCoreApiBroker.DeleteSourceByIdAsync(inputSource.Id);

            ValueTask<Source> getSourceByIdTask =
                this.gitFyleCoreApiBroker.GetSourceByIdAsync(inputSource.Id);

            // then
            deletedSource.Should().BeEquivalentTo(expectedSource);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(() =>
               getSourceByIdTask.AsTask());
        }
    }
}
