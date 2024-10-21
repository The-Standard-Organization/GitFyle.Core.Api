// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveContributionTypeByIdAsync()
        {
            // given
            ContributionType randomContributionType = CreateRandomContributionType();
            ContributionType storageContributionType = randomContributionType;
            ContributionType expectedContributionType = storageContributionType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionTypeByIdAsync(randomContributionType.Id))
                    .ReturnsAsync(storageContributionType);

            // when
            ContributionType actualContributionType =
                await this.contributionTypeService.RetrieveContributionTypeByIdAsync(
                    randomContributionType.Id);

            // then
            actualContributionType.Should().BeEquivalentTo(expectedContributionType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(randomContributionType.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}