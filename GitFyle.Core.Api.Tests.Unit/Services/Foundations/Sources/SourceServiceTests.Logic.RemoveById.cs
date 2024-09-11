// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldRemoveSourceByIdAsync()
        {
            // given
            Guid someSourceId = Guid.NewGuid();
            Source randomSource = CreateRandomSource();
            Source storageSource = randomSource;
            Source inputSource = storageSource;
            Source removedSource = inputSource;
            Source expectedSource = removedSource.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(someSourceId))
                    .ReturnsAsync(storageSource);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteSourceAsync(inputSource))
                    .ReturnsAsync(removedSource);

            // when
            Source actualSource =
                await this.sourceService.RemoveSourceByIdAsync(someSourceId);

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(someSourceId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSourceAsync(storageSource),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}