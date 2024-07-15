// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.InstructionCounters
{
    public partial class InstructionCounterTests
    {
        [Fact]
        public async Task ShouldAddSourceWithoutCounterAsync()
        {
            await MustCompleteWithinTimeout(async () =>
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
                    broker.InsertSourceAsync(inputSource),
                        Times.Once);

                this.storageBrokerMock.VerifyNoOtherCalls();
                this.loggingBrokerMock.VerifyNoOtherCalls();
            },
            timeoutMilliseconds: 1000);
        }
    }
}
