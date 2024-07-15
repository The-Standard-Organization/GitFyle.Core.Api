// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.RecursiveInstructionCounters
{
    public partial class RecursiveInstructionCounterTests
    {
        [Fact]
        public async Task NoCounterAsync()
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
        }

        [Fact]
        public void NoCounter()
        {
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

            // then
            actualSources.Should().BeEquivalentTo(expectedSources);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSources(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
