// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributorIsNullAndLogItAsync()
        {
            // given
            Contributor nullContributor = null;

            var nullContributorException =
                new NullContributorException(message: "Contributor is null");

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: nullContributorException);

            // when
            ValueTask<Contributor> addContributorTask =
                this.contributorService.ModifyContributorAsync(nullContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: addContributorTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributorValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributorAsync(It.IsAny<Contributor>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributorIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            var invalidContributor = new Contributor
            {
                Id = Guid.Empty,
                SourceId = Guid.Empty,
                Username = invalidString,
                Name = invalidString,
                Email = invalidString,
                AvatarUrl = invalidString,
                CreatedBy = invalidString,
                CreatedDate = default,
                UpdatedBy = invalidString,
                UpdatedDate = default,
            };

            var invalidContributorException = new InvalidContributorException(
                message: "Contributor is invalid, fix the errors and try again.");

            invalidContributorException.AddData(
                key: nameof(Contributor.Id),
                values: "Id is invalid");

            invalidContributorException.AddData(
                 key: nameof(Contributor.SourceId),
                 values: "Id is invalid");

            invalidContributorException.AddData(
                key: nameof(Contributor.Username),
                values: "Text is required");

            invalidContributorException.AddData(
                key: nameof(Contributor.Name),
                values: "Text is required");

            invalidContributorException.AddData(
               key: nameof(Contributor.Email),
               values: "Text is required");

            invalidContributorException.AddData(
               key: nameof(Contributor.AvatarUrl),
               values: "Text is required");

            invalidContributorException.AddData(
               key: nameof(Contributor.CreatedBy),
               values: "Text is required");

            invalidContributorException.AddData(
                key: nameof(Contributor.UpdatedBy),
                values: "Text is required");

            invalidContributorException.AddData(
                key: nameof(Contributor.CreatedDate),
                values: "Date is invalid");

            invalidContributorException.AddData(
                key: nameof(Contributor.UpdatedDate),
                values:
                    new[]
                    {
                      "Date is invalid",
                      $"Date is the same as {nameof(Contributor.CreatedDate)}"
                    });

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: invalidContributorException);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(invalidContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributorValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributorAsync(It.IsAny<Contributor>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfContributorHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidContributor = CreateRandomContributor(dateTimeOffset: randomDateTimeOffset);
            invalidContributor.ExternalId = GetRandomStringWithLengthOf(256);
            invalidContributor.Username = GetRandomStringWithLengthOf(256);
            invalidContributor.Name = GetRandomStringWithLengthOf(256);
            invalidContributor.Email = GetRandomStringWithLengthOf(256);

            var invalidContributorException =
                new InvalidContributorException(
                    message: "Contributor is invalid, fix the errors and try again.");

            invalidContributorException.AddData(
                key: nameof(Contributor.ExternalId),
                values: $"Text exceeds max length of {invalidContributor.ExternalId.Length - 1} characters");

            invalidContributorException.AddData(
                key: nameof(Contributor.Username),
                values: $"Text exceeds max length of {invalidContributor.Username.Length - 1} characters");

            invalidContributorException.AddData(
                key: nameof(Contributor.Name),
                values: $"Text exceeds max length of {invalidContributor.Name.Length - 1} characters");

            invalidContributorException.AddData(
                key: nameof(Contributor.Email),
                values: $"Text exceeds max length of {invalidContributor.Email.Length - 1} characters");

            invalidContributorException.AddData(
                key: nameof(Contributor.UpdatedDate),
                values: $"Date is the same as {nameof(Contributor.CreatedDate)}");

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: invalidContributorException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(invalidContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorValidationException.Should()
                .BeEquivalentTo(expectedContributorValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributorAsync(It.IsAny<Contributor>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Contributor randomContributor = CreateRandomContributor(randomDateTimeOffset);
            Contributor invalidContributor = randomContributor;

            var invalidContributorException = new InvalidContributorException(
                message: "Contributor is invalid, fix the errors and try again.");

            invalidContributorException.AddData(
                key: nameof(Contributor.UpdatedDate),
                values: $"Date is the same as {nameof(Contributor.CreatedDate)}");

            var expectedContributorValidationException = new ContributorValidationException(
                message: "Contributor validation error occurred, fix errors and try again.",
                innerException: invalidContributorException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(invalidContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributorValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(It.IsAny<Guid>()),
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
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Contributor randomContributor = CreateRandomContributor(randomDateTimeOffset);
            randomContributor.UpdatedDate = randomDateTimeOffset.AddSeconds(invalidSeconds);

            var invalidContributorException = new InvalidContributorException(
                message: "Contributor is invalid, fix the errors and try again.");

            invalidContributorException.AddData(
                key: nameof(Contributor.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                  $" Expected a value between {startDate} and {endDate} but found {randomContributor.UpdatedDate}"
                ]);

            var expectedContributorValidationException = new ContributorValidationException(
                message: "Contributor validation error occurred, fix errors and try again.",
                innerException: invalidContributorException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(randomContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributorValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageContributorDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegative = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Contributor randomContributor = CreateRandomContributor(randomDateTimeOffset);
            Contributor nonExistingContributor = randomContributor;
            nonExistingContributor.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegative);
            Contributor nullContributor = null;

            var notFoundContributorException =
                new NotFoundContributorException(
                    message: $"Contributor not found with id: {nonExistingContributor.Id}");

            var expectedContributorValidationException =
                new ContributorValidationException(
                    message: "Contributor validation error occurred, fix errors and try again.",
                    innerException: notFoundContributorException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(nonExistingContributor.Id))
                    .ReturnsAsync(nullContributor);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(nonExistingContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(nonExistingContributor.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedAuditInfoHasChangedAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Contributor randomContributor = CreateRandomModifyContributor(randomDateTimeOffset);
            Contributor invalidContributor = randomContributor;
            Contributor storedContributor = randomContributor.DeepClone();
            storedContributor.CreatedBy = GetRandomString();
            storedContributor.CreatedDate = storedContributor.CreatedDate.AddMinutes(randomMinutes);
            storedContributor.UpdatedDate = storedContributor.UpdatedDate.AddMinutes(randomMinutes);
            Guid ContributorId = invalidContributor.Id;

            var invalidContributorException = new InvalidContributorException(
                message: "Contributor is invalid, fix the errors and try again.");

            invalidContributorException.AddData(
                key: nameof(Contributor.CreatedBy),
                values: $"Text is not the same as {nameof(Contributor.CreatedBy)}");

            invalidContributorException.AddData(
                key: nameof(Contributor.CreatedDate),
                values: $"Date is not the same as {nameof(Contributor.CreatedDate)}");

            var expectedContributorValidationException = new ContributorValidationException(
                message: "Contributor validation error occurred, fix errors and try again.",
                innerException: invalidContributorException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(ContributorId))
                    .ReturnsAsync(storedContributor);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(invalidContributor);

            ContributorValidationException actualContributorValidationException =
                await Assert.ThrowsAsync<ContributorValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorValidationException.Should().BeEquivalentTo(
                expectedContributorValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(invalidContributor.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributorValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}