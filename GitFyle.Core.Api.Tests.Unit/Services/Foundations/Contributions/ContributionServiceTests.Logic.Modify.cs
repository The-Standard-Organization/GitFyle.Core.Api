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
        public async Task ShouldModifyContributionAsync()
        {
            //given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();

            Contribution randomModifyContribution =
                CreateRandomModifyContribution(randomDateOffset);

            Contribution inputContribution = randomModifyContribution.DeepClone();
            Contribution storageContribution = randomModifyContribution.DeepClone();
            storageContribution.UpdatedDate = storageContribution.CreatedDate;
            Contribution updatedContribution = inputContribution.DeepClone();
            Contribution expectedContribution = updatedContribution.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(inputContribution.Id))
                    .ReturnsAsync(storageContribution);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateContributionAsync(inputContribution))
                    .ReturnsAsync(updatedContribution);

            //when
            Contribution actualContribution =
                await this.contributionService.ModifyContributionAsync(inputContribution);

            //then
            actualContribution.Should().BeEquivalentTo(expectedContribution);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(inputContribution.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributionAsync(inputContribution),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
