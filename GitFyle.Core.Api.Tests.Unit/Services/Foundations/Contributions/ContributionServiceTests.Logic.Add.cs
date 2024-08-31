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
        public async Task ShouldAddContributionAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Contribution randomContribution = CreateRandomContribution(dateTimeOffset: now);
            Contribution inputContribution = randomContribution;
            Contribution insertedContribution = inputContribution.DeepClone();
            Contribution expectedContribution = insertedContribution.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertContributionAsync(inputContribution))
                    .ReturnsAsync(insertedContribution);

            // when
            Contribution actualContribution =
                await this.contributionService.AddContributionAsync(inputContribution);

            // then
            actualContribution.Should().BeEquivalentTo(expectedContribution);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionAsync(inputContribution),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
