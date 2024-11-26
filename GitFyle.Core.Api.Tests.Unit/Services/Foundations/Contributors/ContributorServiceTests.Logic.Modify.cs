// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldModifyContributorAsync()
        {
            // given
            DateTimeOffset randomDateOffset = GetRandomDateTimeOffset();

            Contributor randomModifyContributor =
                CreateRandomModifyContributor(randomDateOffset);

            Contributor inputContributor = randomModifyContributor.DeepClone();
            Contributor storageContributor = randomModifyContributor.DeepClone();
            storageContributor.UpdatedDate = storageContributor.CreatedDate;
            Contributor updatedContributor = inputContributor.DeepClone();
            Contributor expectedContributor = updatedContributor.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(inputContributor.Id))
                    .ReturnsAsync(storageContributor);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateContributorAsync(inputContributor))
                    .ReturnsAsync(updatedContributor);

            // when
            Contributor actualContributor =
                await this.contributorService.ModifyContributorAsync(inputContributor);

            // then
            actualContributor.Should().BeEquivalentTo(expectedContributor);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(inputContributor.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributorAsync(inputContributor),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
