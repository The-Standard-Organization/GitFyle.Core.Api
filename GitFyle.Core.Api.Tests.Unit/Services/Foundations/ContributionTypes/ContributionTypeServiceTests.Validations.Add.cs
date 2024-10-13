// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfContributionTypeIsNullAndLogItAsync()
        {
            // given
            ContributionType nullContributionType = null;

            var nullContributionTypeException =
                new NullContributionTypeException(
                    message: "ContributionType is null");

            var expectedContributionTypeValidationException =
                new ContributionTypeValidationException(
                    message: "ContributionType validation error occurred, fix errors and try again.",
                    innerException: nullContributionTypeException);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionService.AddContributionTypeAsync(nullContributionType);

            ContributionTypeValidationException actualContributionTypeValidationException =
                await Assert.ThrowsAsync<ContributionTypeValidationException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeValidationException.Should().BeEquivalentTo(
                expectedContributionTypeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfContributionTypeIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            DateTimeOffset randomDateTimeOffset = default;

            var invalidContributionType = new ContributionType
            {
                Id = Guid.Empty,
                Name = invalidString,
                CreatedBy = invalidString,
                CreatedDate = default,
                UpdatedBy = invalidString,
                UpdatedDate = default,
            };

            var invalidContributionTypeException = new InvalidContributionTypeException(
                message: "ContributionType is invalid, fix the errors and try again.");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.Id),
                values: "Id is invalid");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.Name),
                values: "Text is required");

            invalidContributionTypeException.AddData(
               key: nameof(ContributionType.CreatedBy),
               values: "Text is required");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.UpdatedBy),
                values: "Text is required");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.CreatedDate),
                values: "Date is invalid");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.UpdatedDate),
                values: "Date is invalid");

            var expectedContributionTypeValidationException =
                new ContributionTypeValidationException(
                    message: "ContributionType validation error occurred, fix errors and try again.",
                    innerException: invalidContributionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionService.AddContributionTypeAsync(invalidContributionType);

            ContributionTypeValidationException actualContributionTypeValidationException =
                await Assert.ThrowsAsync<ContributionTypeValidationException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeValidationException.Should().BeEquivalentTo(
                expectedContributionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfContributionTypeHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidContributionType = CreateRandomContributionType(dateTimeOffset: randomDateTimeOffset);
            invalidContributionType.Name = GetRandomStringWithLengthOf(256);

            var invalidContributionTypeException =
                new InvalidContributionTypeException(
                    message: "ContributionType is invalid, fix the errors and try again.");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.Name),
                values: $"Text exceeds max length of {invalidContributionType.Name.Length - 1} characters");

            var expectedContributionTypeValidationException =
                new ContributionTypeValidationException(
                    message: "ContributionType validation error occurred, fix errors and try again.",
                    innerException: invalidContributionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionService.AddContributionTypeAsync(invalidContributionType);

            ContributionTypeValidationException actualContributionTypeValidationException =
                await Assert.ThrowsAsync<ContributionTypeValidationException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeValidationException.Should()
                .BeEquivalentTo(expectedContributionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            ContributionType randomContributionType = CreateRandomContributionType(now);
            ContributionType invalidContributionType = randomContributionType;
            invalidContributionType.CreatedBy = GetRandomString();
            invalidContributionType.UpdatedBy = GetRandomString();
            invalidContributionType.CreatedDate = now;
            invalidContributionType.UpdatedDate = GetRandomDateTimeOffset();

            var invalidContributionTypeException = new InvalidContributionTypeException(
                message: "ContributionType is invalid, fix the errors and try again.");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.UpdatedBy),
                values: $"Text is not the same as {nameof(ContributionType.CreatedBy)}");

            invalidContributionTypeException.AddData(
                key: nameof(ContributionType.UpdatedDate),
                values: $"Date is not the same as {nameof(ContributionType.CreatedDate)}");

            var expectedContributionTypeValidationException =
                new ContributionTypeValidationException(
                    message: "ContributionType validation error occurred, fix errors and try again.",
                    innerException: invalidContributionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionService.AddContributionTypeAsync(invalidContributionType);

            ContributionTypeValidationException actualContributionTypeValidationException =
                await Assert.ThrowsAsync<ContributionTypeValidationException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeValidationException.Should().BeEquivalentTo(
                expectedContributionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidSeconds)
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            DateTimeOffset now = randomDateTime;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            ContributionType randomContributionType = CreateRandomContributionType();
            ContributionType invalidContributionType = randomContributionType;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidContributionType.CreatedDate = invalidDate;
            invalidContributionType.UpdatedDate = invalidDate;

            var invalidContributionTypeException = new InvalidContributionTypeException(
                message: "ContributionType is invalid, fix the errors and try again.");

            invalidContributionTypeException.AddData(
            key: nameof(ContributionType.CreatedDate),
                values:
                    $"Date is not recent. Expected a value between " +
                    $"{startDate} and {endDate} but found {invalidDate}");

            var expectedContributionTypeValidationException =
                new ContributionTypeValidationException(
                    message: "ContributionType validation error occurred, fix errors and try again.",
                    innerException: invalidContributionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionService.AddContributionTypeAsync(invalidContributionType);

            ContributionTypeValidationException actualContributionTypeValidationException =
                await Assert.ThrowsAsync<ContributionTypeValidationException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeValidationException.Should().BeEquivalentTo(
                expectedContributionTypeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedContributionTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
