// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Moq;
using Valid8R;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldAddSourceAsync()
        {
            // given
            Source randomSource = CreateRandomSource();
            Source inputSource = randomSource;
            Source insertedSource = inputSource.DeepClone();
            Source expectedSource = insertedSource.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSourceAsync(inputSource))
                    .ReturnsAsync(insertedSource);

            // when
            Source actualSource =
                await this.sourceService.AddSourceAsync(inputSource);

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSourceAsync(
                    It.Is(Valid8.SameObjectAs<Source>(inputSource, output, "1st this.storageBrokerMock.Verify"))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
