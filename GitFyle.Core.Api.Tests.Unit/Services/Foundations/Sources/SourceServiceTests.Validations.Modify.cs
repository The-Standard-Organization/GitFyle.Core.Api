// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Moq;
using System;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfSourceIsNullAndLogItAsync()
        {
            //given
            Source nullSource = null;

            var nullSourceException =
                new NullSourceException(message: "Source is null");

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: nullSourceException);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(nullSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedSourceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(It.IsAny<Source>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfSourceIsInvalidAndLogItAsync(string invalidString)
        {
            //given
            var invalidSource = new Source
            {
                Name = invalidString,
                Url = invalidString,
                UpdatedBy = invalidString,
            };

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.Id),
                values: "Id is invalid");

            invalidSourceException.AddData(
                key: nameof(Source.Name),
                values: "Text is required");

            invalidSourceException.AddData(
                key: nameof(Source.Url),
                values: "Url is invalid");

            invalidSourceException.AddData(
                key: nameof(Source.CreatedBy),
                values: "Text is required");

            invalidSourceException.AddData(
                key: nameof(Source.UpdatedBy),
                values: "Text is required");

            invalidSourceException.AddData(
                key: nameof(Source.CreatedDate),
                values: "Date is invalid");

            invalidSourceException.AddData(
                key: nameof(Source.UpdatedDate),
                values:
                    new[]
                    {
                        "Date is invalid",
                        $"Date is the same as {nameof(Source.CreatedDate)}"
                    });

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: invalidSourceException);

            // when
            ValueTask<Source> modifySourceTask =
                this.sourceService.ModifySourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    modifySourceTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedSourceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(It.IsAny<Source>()),
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
            Source randomSource = CreateRandomSource(randomDateTimeOffset);
            Source invalidSource = randomSource;

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.UpdatedDate),
                values: $"Date is the same as {nameof(Source.CreatedDate)}");

            var expectedSourceValidationException = new SourceValidationException(
                message: "Source validation error occurred, fix errors and try again.",
                innerException: invalidSourceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Source> modifySourceTask =
                this.sourceService.ModifySourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    modifySourceTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedSourceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinuteCases))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            //given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTimeOffset;
            DateTimeOffset startDate = now.AddSeconds(-60);
            DateTimeOffset endDate = now.AddSeconds(0);
            Source randomSource = CreateRandomSource(randomDateTimeOffset);
            randomSource.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.UpdatedDate),
                values:
                [
                    $"Date is not recent." +
                    $" Expected a value between {startDate} and {endDate} but found {randomSource.UpdatedDate}"
                ]);

            var expectedSourceValidationException = new SourceValidationException(
                message: "Source validation error occurred, fix errors and try again.",
                innerException: invalidSourceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Source> modifySourceTask =
                this.sourceService.ModifySourceAsync(randomSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    modifySourceTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(
                    SameExceptionAs(expectedSourceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}