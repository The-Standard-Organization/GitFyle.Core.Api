// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Tests.Unit.Helpers;
using Mono.Cecil;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.InstructionCounters
{
    public partial class InstructionCounterTests
    {

        [Fact]
        public async Task ShouldAddSourceWithInstructionCounterAsync()
        {
            await MustCompleteWithinTimeout(async () =>
            {
                // given
                int expectedNumberOfInstructions = 14;

                MethodDefinition methodDefinition =
                    FindMethodDefinition(methodDelegate: this.sourceService.AddSourceAsync);

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

                int actualNumberOfInstructions =
                    GetInstructionCount(methodDefinition);

                // then
                actualSource.Should().BeEquivalentTo(expectedSource);
                actualNumberOfInstructions.Should().Be(expectedNumberOfInstructions);

                this.storageBrokerMock.Verify(broker =>
                    broker.InsertSourceAsync(inputSource),
                        Times.Once);

                this.storageBrokerMock.VerifyNoOtherCalls();
                this.loggingBrokerMock.VerifyNoOtherCalls();
            },
            timeoutMilliseconds: 1000);
        }

        [Fact]
        public async Task ShouldAddSourceWithInstructionCounterIncludingTryCatchAsync()
        {
            // given
            int expectedNumberOfInstructions = 33;

            InstructionCounter instructionCounter = new InstructionCounter(
                methodDelegate: this.sourceService.AddSourceAsync);

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

            int actualNumberOfInstructions = instructionCounter.CountInstructionsIncludingTryCatch();

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);
            actualNumberOfInstructions.Should().Be(expectedNumberOfInstructions);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(inputSource),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldAddSourceWithRecursiveInstructionCounterAsync()
        {
            // given
            int expectedNumberOfInstructions = 91233;

            InstructionCounter instructionCounter = new InstructionCounter(
                methodDelegate: this.sourceService.AddSourceAsync);

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

            int actualNumberOfInstructions = instructionCounter.CountInstructionsRecursive();

            // then
            actualSource.Should().BeEquivalentTo(expectedSource);
            actualNumberOfInstructions.Should().Be(expectedNumberOfInstructions);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(inputSource),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
