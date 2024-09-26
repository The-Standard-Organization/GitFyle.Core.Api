// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllContributionsAsync()
        {
            // given
            IQueryable<Contribution> randomContributions = CreateRandomContributions();
            IQueryable<Contribution> storageContributions = randomContributions;
            IQueryable<Contribution> expectedContributions = storageContributions;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllContributionsAsync())
                    .ReturnsAsync(storageContributions);

            // when
            IQueryable<Contribution> actualContributions =
                await this.contributionService.RetrieveAllContributionsAsync();

            // then
            actualContributions.Should().BeEquivalentTo(expectedContributions);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllContributionsAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}