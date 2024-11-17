// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveContributorByIdAsync()
        {
            // given
            Contributor randomContributor = CreateRandomContributor();
            Contributor storageContributor = randomContributor;
            Contributor expectedContributor = storageContributor.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(randomContributor.Id))
                    .ReturnsAsync(storageContributor);

            // when
            Contributor actualContributor =
                await this.contributorService.RetrieveContributorByIdAsync(
                    randomContributor.Id);

            // then
            actualContributor.Should().BeEquivalentTo(expectedContributor);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(randomContributor.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}