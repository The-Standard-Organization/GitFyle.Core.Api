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

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.RecursiveInstructionCounters
{
    public partial class RecursiveInstructionCounterTests
    {

        [Fact]
        public async Task BasicRecursiveInstructionCounterAsync()
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
        }

        [Fact]
        public async Task RecursiveInstructionCounterWithTryCatchAsync()
        {
            // given
            int expectedNumberOfInstructions = 91237;

            RecursiveInstructionCounter instructionCounter = new RecursiveInstructionCounter(
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

        [Fact]
        public void RecursiveInstructionCounterWithTryCatch()
        {
            // given
            int expectedNumberOfInstructions = 128;

            RecursiveInstructionCounter instructionCounter = new RecursiveInstructionCounter(
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

            int actualNumberOfInstructions = instructionCounter.CountInstructionsRecursive();

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
