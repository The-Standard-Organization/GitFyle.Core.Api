// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldRetrieveSourceByIdAsync()
        {
            // given
            Source randomSource = CreateRandomSource();
            Source storageSource = randomSource;
            Source expectedSource = storageSource.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(randomSource.Id))
                    .ReturnsAsync(storageSource);

            // when
            Source actualSource =
                await this.sourceService.RetrieveSourceByIdAsync(
                    randomSource.Id);

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(randomSource.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}