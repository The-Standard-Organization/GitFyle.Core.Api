// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfContributionIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            var invalidContribution = new Contribution
            {
                Id = Guid.Empty,
                ContributorId = Guid.Empty,
                RepositoryId = Guid.Empty,
                ContributionTypeId = Guid.Empty,
                Title = invalidString,
                ExternalId = invalidString,
                ExternalCreatedAt = default,
                ExternalMergedAt = default,
                ExternalUpdatedAt = default,
            };

            var invalidContributionException = new InvalidContributionException(
                message: "Contribution is invalid, fix the errors and try again.");

            invalidContributionException.AddData(
                key: nameof(Contribution.Id),
                values: "Id is invalid");

            invalidContributionException.AddData(
                key: nameof(Contribution.ContributorId),
                values: "Id is invalid");

            invalidContributionException.AddData(
                key: nameof(Contribution.RepositoryId),
                values: "Id is invalid");

            invalidContributionException.AddData(
                key: nameof(Contribution.ContributionTypeId),
                values: "Id is invalid");

            invalidContributionException.AddData(
                key: nameof(Contribution.ExternalId),
                values: "Text is required");

            invalidContributionException.AddData(
                key: nameof(Contribution.Title),
                values: "Text is required");


            invalidContributionException.AddData(
                key: nameof(Contribution.ExternalCreatedAt),
                values: "Date is invalid");

            invalidContributionException.AddData(
                key: nameof(Contribution.ExternalMergedAt),
                values: "Date is invalid");

            invalidContributionException.AddData(
                key: nameof(Contribution.ExternalUpdatedAt),
                values: "Date is invalid");

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: invalidContributionException);

            // when
            ValueTask<Contribution> addContributionTask =
                this.contributionService.AddContributionAsync(invalidContribution);

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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfContributionHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidContribution = CreateRandomContribution(dateTimeOffset: randomDateTimeOffset);
            invalidContribution.Title = GetRandomStringWithLengthOf(256);
            invalidContribution.ExternalId = GetRandomStringWithLengthOf(256);

            var invalidContributionException =
                new InvalidContributionException(
                    message: "Contribution is invalid, fix the errors and try again.");

            invalidContributionException.AddData(
                key: nameof(Contribution.Title),
                values: $"Text exceed max length of {invalidContribution.Title.Length - 1} characters");

            invalidContributionException.AddData(
                key: nameof(Contribution.ExternalId),
                values: $"Text exceed max length of {invalidContribution.ExternalId.Length - 1} characters");

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: invalidContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contribution> addContributionTask =
                this.contributionService.AddContributionAsync(invalidContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    addContributionTask.AsTask);

            // then
            actualContributionValidationException.Should()
                .BeEquivalentTo(expectedContributionValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionAsync(It.IsAny<Contribution>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }


        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnAddIfExternalCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset now = randomDateTime;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Contribution randomContribution = CreateRandomContribution();
            Contribution invalidContribution = randomContribution;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidContribution.ExternalCreatedAt = invalidDate;
            invalidContribution.ExternalUpdatedAt = invalidDate;

            var invalidContributionException = new InvalidContributionException(
                message: "Contribution is invalid, fix the errors and try again.");

            invalidContributionException.AddData(
            key: nameof(Contribution.ExternalCreatedAt),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: invalidContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Contribution> addContributionTask =
                this.contributionService.AddContributionAsync(invalidContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    addContributionTask.AsTask);

            // then
            actualContributionValidationException.Should().BeEquivalentTo(
                expectedContributionValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionAsync(It.IsAny<Contribution>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
