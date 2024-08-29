// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfContributionIsNullAndLogItAsync()
        {
            // given
            Contribution nullContribution = null;

            var nullContributionException =
                new NullContributionException(
                    message: "Contribution is null");

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: nullContributionException);

            // when
            ValueTask<Contribution> addContributionTask =
                this.contributionService.AddContributionAsync(nullContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    addContributionTask.AsTask);

            // then
            actualContributionValidationException.Should().BeEquivalentTo(
                expectedContributionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionAsync(It.IsAny<Contribution>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
