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
        public async Task ShouldAddContributionTypeAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            ContributionType randomContributionType = CreateRandomContributionType(dateTimeOffset: now);
            ContributionType inputContributionType = randomContributionType;
            ContributionType insertedContributionType = inputContributionType.DeepClone();
            ContributionType expectedContributionType = insertedContributionType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertContributionTypeAsync(inputContributionType))
                    .ReturnsAsync(insertedContributionType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ContributionType actualContributionType =
                await this.contributionTypeService.AddContributionTypeAsync(inputContributionType);

            // then
            actualContributionType.Should().BeEquivalentTo(expectedContributionType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(inputContributionType),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
