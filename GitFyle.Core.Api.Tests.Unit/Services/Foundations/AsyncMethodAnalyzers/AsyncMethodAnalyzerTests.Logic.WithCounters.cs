// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Tests.Unit.Helpers;
using Mono.Cecil;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.AsyncMethodAnalyzers
{
    public partial class AsyncMethodAnalyzerTests
    {

        [Fact]
        public async Task BasicInstructionCounterAsync()
        {
            await MustCompleteWithinTimeout(async () =>
            {
                // given
                int expectedNumberOfInstructions = 39;

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
                    AsyncMethodAnalyzer.AnalyzeAsyncMethod(methodDelegate: this.sourceService.AddSourceAsync);

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
        public async Task InstructionCounterWithTryCatchAsync()
        {
            // given
            int expectedNumberOfInstructions = 39;

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

            int actualNumberOfInstructions =
                AsyncMethodAnalyzer.AnalyzeAsyncMethod(methodDelegate: this.sourceService.AddSourceAsync);

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
        public void InstructionCounterWithTryCatch()
        {
            // given
            int expectedNumberOfInstructions = 39;

            InstructionCounter instructionCounter = new InstructionCounter(
                methodDelegate: this.sourceService.RetrieveAllSources);

            // given
            IQueryable<Source> randomSources = CreateRandomSources();
            IQueryable<Source> storageSources = randomSources;
            IQueryable<Source> expectedSources = storageSources;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSources())
                    .Returns(storageSources);

            // when
            IQueryable<Source> actualSources =
                this.sourceService.RetrieveAllSources();

            int actualNumberOfInstructions =
                AsyncMethodAnalyzer.AnalyzeAsyncMethod(methodDelegate: this.sourceService.AddSourceAsync);

            // then
            actualSources.Should().BeEquivalentTo(expectedSources);
            actualNumberOfInstructions.Should().Be(expectedNumberOfInstructions);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSources(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
