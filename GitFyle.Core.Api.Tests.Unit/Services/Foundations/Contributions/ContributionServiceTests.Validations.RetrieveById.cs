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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdWhenContributionIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidContributionId = Guid.Empty;

            var invalidContributionException =
                new InvalidContributionException(
                    message: "Contribution is invalid, fix the errors and try again.");

            invalidContributionException.AddData(
                key: nameof(Contribution.Id),
                values: "Id is invalid");

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: invalidContributionException);

            // when
            ValueTask<Contribution> retrieveContributionByIdTask =
                this.contributionService.RetrieveContributionByIdAsync(invalidContributionId);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    testCode: retrieveContributionByIdTask.AsTask);

            // then
            actualContributionValidationException.Should().BeEquivalentTo(
                expectedContributionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfContributionIdNotFoundAndLogitAsync()
        {
            // given
            var someContributionId = Guid.NewGuid();
            Contribution nullContribution = null;
            var innerException = new Exception();

            var notFoundContributionException =
                new NotFoundContributionException(
                    message: $"Contribution not found with id: {someContributionId}");

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: notFoundContributionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(someContributionId))
                    .ReturnsAsync(nullContribution);

            // when
            ValueTask<Contribution> retrieveContributionByIdTask =
                this.contributionService.RetrieveContributionByIdAsync(someContributionId);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    testCode: retrieveContributionByIdTask.AsTask);

            // then
            actualContributionValidationException.Should().BeEquivalentTo(
                expectedContributionValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(someContributionId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}