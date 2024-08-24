// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllSourcesAsync()
        {
            // given
            IQueryable<Source> randomSources = CreateRandomSources();
            IQueryable<Source> storageSources = randomSources;
            IQueryable<Source> expectedSources = storageSources;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSourcesAsync())
                    .ReturnsAsync(storageSources);

            // when
            IQueryable<Source> actualSources =
                await this.sourceService.RetrieveAllSourcesAsync();

            // then
            actualSources.Should().BeEquivalentTo(expectedSources);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSourcesAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}