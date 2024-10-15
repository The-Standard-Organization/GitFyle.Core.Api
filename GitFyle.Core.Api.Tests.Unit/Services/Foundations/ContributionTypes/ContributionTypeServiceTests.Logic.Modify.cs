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
        public async Task ShouldModifyContributionTypeAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();

            ContributionType randomModifyContributionType =
                CreateRandomModifyContributionType(randomDateOffset);

            ContributionType inputContributionType = randomModifyContributionType.DeepClone();
            ContributionType storageContributionType = randomModifyContributionType.DeepClone();
            storageContributionType.UpdatedDate = storageContributionType.CreatedDate;
            ContributionType updatedContributionType = inputContributionType.DeepClone();
            ContributionType expectedContributionType = updatedContributionType.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionTypeByIdAsync(inputContributionType.Id))
                    .ReturnsAsync(storageContributionType);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateContributionTypeAsync(inputContributionType))
                    .ReturnsAsync(updatedContributionType);

            // when
            ContributionType actualContributionType =
                await this.contributionTypeService.ModifyContributionTypeAsync(inputContributionType);

            // then
            actualContributionType.Should().BeEquivalentTo(expectedContributionType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(inputContributionType.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributionTypeAsync(inputContributionType),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
