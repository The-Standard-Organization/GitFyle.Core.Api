// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRemoveContributionTypeByIdAsync()
        {
            // given
            Guid someContributionTypeId = Guid.NewGuid();
            ContributionType randomContributionType = CreateRandomContributionType();
            ContributionType storageContributionType = randomContributionType;
            ContributionType inputContributionType = storageContributionType;
            ContributionType removedContributionType = inputContributionType;
            ContributionType expectedContributionType = removedContributionType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionTypeByIdAsync(someContributionTypeId))
                    .ReturnsAsync(storageContributionType);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteContributionTypeAsync(inputContributionType))
                    .ReturnsAsync(removedContributionType);

            // when
            ContributionType actualContributionType =
                await this.contributionTypeService.RemoveContributionTypeByIdAsync(someContributionTypeId);

            // then
            actualContributionType.Should().BeEquivalentTo(expectedContributionType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(someContributionTypeId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteContributionTypeAsync(storageContributionType),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}