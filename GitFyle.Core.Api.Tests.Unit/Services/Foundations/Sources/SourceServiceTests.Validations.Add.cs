// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfSourceIsNullAndLogItAsync()
        {
            // given
            Source nullSource = null;

            var nullSourceException =
                new NullSourceException(
                    message: "Source is null");

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
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfSourceIsInvalidAndLogItAsync(
            string invalidString)
        {
            // given
            var invalidSource = new Source
            {
                Id = Guid.Empty,
                Name = invalidString,
                Url = invalidString,
                CreatedBy = invalidString,
                UpdatedBy = invalidString,
                CreatedDate = default,
                UpdatedDate = default
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
                values: "Date is invalid");

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: invalidSourceException);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

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
        public async Task ShouldThrowValidationExceptionOnAddIfSourceHasInvalidLengthPropertiesAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            var invalidSource = CreateRandomSource(dateTimeOffset: randomDateTimeOffset);
            invalidSource.Name = GetRandomStringWithLengthOf(256);

            var invalidSourceException =
                new InvalidSourceException(
                    message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.Name),
                values: $"Text exceed max length of {invalidSource.Name.Length - 1} characters");

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: invalidSourceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

            // then
            actualSourceValidationException.Should()
                .BeEquivalentTo(expectedSourceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSourceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(It.IsAny<Source>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfUrlIsInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Source randomSource = CreateRandomSource(now);
            Source invalidSource = randomSource;
            string invalidUrl = GetRandomString();
            invalidSource.Url = invalidUrl;

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.Url),
                values: "Url is invalid");

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: invalidSourceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

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
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset now = randomDateTime;
            Source randomSource = CreateRandomSource(now);
            Source invalidSource = randomSource;
            invalidSource.CreatedBy = GetRandomString();
            invalidSource.UpdatedBy = GetRandomString();
            invalidSource.CreatedDate = now;
            invalidSource.UpdatedDate = GetRandomDateTimeOffset();

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.UpdatedBy),
                values: $"Text is not the same as {nameof(Source.CreatedBy)}");

            invalidSourceException.AddData(
                key: nameof(Source.UpdatedDate),
                values: $"Date is not the same as {nameof(Source.CreatedDate)}");

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: invalidSourceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

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
            Source randomSource = CreateRandomSource();
            Source invalidSource = randomSource;

            DateTimeOffset invalidDate =
                now.AddSeconds(invalidSeconds);

            invalidSource.CreatedDate = invalidDate;
            invalidSource.UpdatedDate = invalidDate;

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.CreatedDate),
                values: $"Date is not recent");

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: invalidSourceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

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
    }
}
