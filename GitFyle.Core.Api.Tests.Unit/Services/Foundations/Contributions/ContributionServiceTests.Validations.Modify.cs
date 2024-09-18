// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributionIsNullAndLogItAsync()
        {
            //given
            Contribution nullContribution = null;

            var nullContributionException =
                new NullContributionException(message: "Contribution is null");

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
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributionIsInvalidAndLogItAsync(
                string invalidString)
        {
            //given
            var invalidContribution = new Contribution
            {
                Id = Guid.Empty,
                RepositoryId = Guid.Empty,
                ContributorId = Guid.Empty,
                ContributionTypeId = Guid.Empty,
                ExternalId = invalidString,
                Title = invalidString,
                CreatedBy = invalidString,
                CreatedDate = default,
                UpdatedBy = invalidString,
                UpdatedDate = default,
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
               key: nameof(Contribution.CreatedBy),
               values: "Text is required");

            invalidContributionException.AddData(
                key: nameof(Contribution.UpdatedBy),
                values: "Text is required");

            invalidContributionException.AddData(
                key: nameof(Contribution.CreatedDate),
                values: "Date is invalid");

            invalidContributionException.AddData(
                key: nameof(Contribution.UpdatedDate),
                values:
                    new[]
                    {
                        "Date is invalid",
                        $"Date is the same as {nameof(Contribution.CreatedDate)}"
                    });

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: invalidContributionException);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(invalidContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    modifyContributionTask.AsTask);

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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributionHasInvalidLengthPropertiesAndLogItAsync()
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

            invalidContributionException.AddData(
                key: nameof(Contribution.UpdatedDate),
                values: $"Date is the same as {nameof(Contribution.CreatedDate)}");

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: invalidContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(invalidContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    modifyContributionTask.AsTask);

            // then
            actualContributionValidationException.Should()
                .BeEquivalentTo(expectedContributionValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionAsync(It.IsAny<Contribution>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Contribution randomContribution = CreateRandomContribution(randomDateTimeOffset);
            Contribution invalidContribution = randomContribution;

            var invalidContributionException = new InvalidContributionException(
                message: "Contribution is invalid, fix the errors and try again.");

            invalidContributionException.AddData(
                key: nameof(Contribution.UpdatedDate),
                values: $"Date is the same as {nameof(Contribution.CreatedDate)}");

            var expectedContributionValidationException = new ContributionValidationException(
                message: "Contribution validation error occurred, fix errors and try again.",
                innerException: invalidContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(invalidContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    modifyContributionTask.AsTask);

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
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
         int invalidSeconds)
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Contribution randomContribution = CreateRandomContribution(randomDateTimeOffset);
            randomContribution.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidContributionException = new InvalidContributionException(
                message: "Contribution is invalid, fix the errors and try again.");

            invalidContributionException.AddData(
                key: nameof(Contribution.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {randomContribution.UpdatedDate}"
                ]);

            var expectedContributionValidationException = new ContributionValidationException(
                message: "Contribution validation error occurred, fix errors and try again.",
                innerException: invalidContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(randomContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    modifyContributionTask.AsTask);

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
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageContributionDoesNotExistAndLogItAsync()
        {
            //given
            int randomNegative = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Contribution randomContribution = CreateRandomContribution(randomDateTimeOffset);
            Contribution nonExistingContribution = randomContribution;
            nonExistingContribution.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            Contribution nullContribution = null;

            var notFoundContributionException =
                new NotFoundContributionException(
                    message: $"Contribution not found with id: {nonExistingContribution.Id}");

            var expectedContributionValidationException =
                new ContributionValidationException(
                    message: "Contribution validation error occurred, fix errors and try again.",
                    innerException: notFoundContributionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(nonExistingContribution.Id))
                    .ReturnsAsync(nullContribution);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(nonExistingContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    modifyContributionTask.AsTask);

            // then
            actualContributionValidationException.Should().BeEquivalentTo(
                expectedContributionValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(nonExistingContribution.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            //given
            int randomMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Contribution randomContribution = CreateRandomModifyContribution(randomDateTimeOffset);
            Contribution invalidContribution = randomContribution;
            Contribution storedContribution = randomContribution.DeepClone();
            storedContribution.CreatedBy = GetRandomString();
            storedContribution.CreatedDate = storedContribution.CreatedDate.AddMinutes(randomMinutes);
            storedContribution.UpdatedDate = storedContribution.UpdatedDate.AddMinutes(randomMinutes);
            Guid ContributionId = invalidContribution.Id;

            var invalidContributionException = new InvalidContributionException(
                message: "Contribution is invalid, fix the errors and try again.");

            invalidContributionException.AddData(
                key: nameof(Contribution.CreatedBy),
                values: $"Text is not the same as {nameof(Contribution.CreatedBy)}");

            invalidContributionException.AddData(
                key: nameof(Contribution.CreatedDate),
                values: $"Date is not the same as {nameof(Contribution.CreatedDate)}");

            var expectedContributionValidationException = new ContributionValidationException(
                message: "Contribution validation error occurred, fix errors and try again.",
                innerException: invalidContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(ContributionId))
                    .ReturnsAsync(storedContribution);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(invalidContribution);

            ContributionValidationException actualContributionValidationException =
                await Assert.ThrowsAsync<ContributionValidationException>(
                    modifyContributionTask.AsTask);

            // then
            actualContributionValidationException.Should().BeEquivalentTo(
                expectedContributionValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(invalidContribution.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}