// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllContributorsAsync()
        {
            // given
            IQueryable<Contributor> randomContributors = CreateRandomContributors();
            IQueryable<Contributor> storageContributors = randomContributors;
            IQueryable<Contributor> expectedContributors = storageContributors;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllContributorsAsync())
                    .ReturnsAsync(storageContributors);

            // when
            IQueryable<Contributor> actualContributors =
                await this.contributorService.RetrieveAllContributorsAsync();

            // then
            actualContributors.Should().BeEquivalentTo(expectedContributors);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllContributorsAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}