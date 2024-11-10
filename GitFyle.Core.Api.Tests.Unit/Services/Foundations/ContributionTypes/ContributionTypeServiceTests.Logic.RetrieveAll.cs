// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllContributionTypesAsync()
        {
            // given
            IQueryable<ContributionType> randomContributionTypes = CreateRandomContributionTypes();
            IQueryable<ContributionType> storageContributionTypes = randomContributionTypes;
            IQueryable<ContributionType> expectedContributionTypes = storageContributionTypes;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllContributionTypesAsync())
                    .ReturnsAsync(storageContributionTypes);

            // when
            IQueryable<ContributionType> actualContributionTypes =
                await this.contributionTypeService.RetrieveAllContributionTypesAsync();

            // then
            actualContributionTypes.Should().BeEquivalentTo(expectedContributionTypes);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllContributionTypesAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}