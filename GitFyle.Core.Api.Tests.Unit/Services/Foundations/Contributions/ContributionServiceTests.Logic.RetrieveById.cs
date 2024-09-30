// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveContributionByIdAsync()
        {
            // given
            Contribution randomContribution = CreateRandomContribution();
            Contribution storageContribution = randomContribution;
            Contribution expectedContribution = storageContribution.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(randomContribution.Id))
                    .ReturnsAsync(storageContribution);

            // when
            Contribution actualContribution =
                await this.contributionService.RetrieveContributionByIdAsync(
                    randomContribution.Id);

            // then
            actualContribution.Should().BeEquivalentTo(expectedContribution);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(randomContribution.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}