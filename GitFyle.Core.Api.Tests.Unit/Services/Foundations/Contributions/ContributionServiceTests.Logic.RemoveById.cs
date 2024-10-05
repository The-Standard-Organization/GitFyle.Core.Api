// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRemoveContributionByIdAsync()
        {
            // given
            Guid someContributionId = Guid.NewGuid();
            Contribution randomContribution = CreateRandomContribution();
            Contribution storageContribution = randomContribution;
            Contribution inputContribution = storageContribution;
            Contribution removedContribution = inputContribution;
            Contribution expectedContribution = removedContribution.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(someContributionId))
                    .ReturnsAsync(storageContribution);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteContributionAsync(inputContribution))
                    .ReturnsAsync(removedContribution);

            // when
            Contribution actualContribution =
                await this.contributionService.RemoveContributionByIdAsync(someContributionId);

            // then
            actualContribution.Should().BeEquivalentTo(expectedContribution);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(someContributionId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteContributionAsync(storageContribution),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}