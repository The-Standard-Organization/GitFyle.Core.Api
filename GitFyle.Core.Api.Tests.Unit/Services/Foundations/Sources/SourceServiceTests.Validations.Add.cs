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
                broker.LogError(It.Is(
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
        public async Task ShouldCallValidateWithTheseExpectedValidationRulesAsync(string invalidString)
        {
            // given
            var invalidSource = new Source
            {
                Id = Guid.Empty,
                Name = invalidString,
                Url = "invalidString",
                CreatedBy = invalidString,
                UpdatedBy = invalidString,
                CreatedDate = default,
                UpdatedDate = default
            };

            (dynamic Rule, string Parameter)[] validationCriteria =
            [
                (
                    Rule: new { Condition = true, Message = "Id is invalid" },
                    Parameter: nameof(Source.Id)
                ),

                (
                    Rule: new { Condition = true, Message = "Text is invalid" },
                    Parameter: nameof(Source.Name)
                ),

                (
                    Rule: new { Condition = true, Message = "Url is invalid" },
                    Parameter: nameof(Source.Url)
                ),

                (
                    Rule: new { Condition = false, Message = "Text exceed max length of 255 characters" },
                    Parameter: nameof(Source.Name)
                ),

                (
                    Rule: new { Condition = true, Message = "Text is invalid" },
                    Parameter: nameof(Source.CreatedBy)
                ),

                (
                    Rule: new { Condition = true, Message = "Text is invalid" },
                    Parameter: nameof(Source.UpdatedBy)
                ),

                (
                    Rule: new { Condition = true, Message = "Date is invalid" },
                    Parameter: nameof(Source.CreatedDate)
                ),

                (
                    Rule: new { Condition = true, Message = "Date is invalid" },
                    Parameter: nameof(Source.UpdatedDate)
                ),


                (
                    Rule: new { Condition = false, Message = $"Text is not the same as {nameof(Source.CreatedBy)}" },
                    Parameter: nameof(Source.UpdatedBy)
                ),

                (
                    Rule: new { Condition = false, Message = $"Date is not the same as {nameof(Source.CreatedDate)}" },
                    Parameter: nameof(Source.UpdatedDate)
                )
            ];

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.Id),
                values: "Id is invalid");

            invalidSourceException.AddData(
                key: nameof(Source.Name),
                values: "Text is invalid");

            invalidSourceException.AddData(
                key: nameof(Source.Url),
                values: "Url is invalid");

            invalidSourceException.AddData(
                key: nameof(Source.CreatedBy),
                values: "Text is invalid");

            invalidSourceException.AddData(
                key: nameof(Source.UpdatedBy),
                values: "Text is invalid");

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

            this.validationBrokerMock
                .Setup(broker =>
                    broker.Validate<InvalidSourceException>(
                        "Source is invalid, fix the errors and try again.",
                        It.IsAny<(dynamic Rule, string Parameter)[]>()))
                .ThrowsAsync(invalidSourceException);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.validationBrokerMock.Verify(broker =>
                broker.Validate<InvalidSourceException>(
                    "Source is invalid, fix the errors and try again.",
                        It.Is(Validations.Comparer.SameRulesAs(validationCriteria, output, "Validation Rules:"))),
                            Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedSourceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(It.IsAny<Source>()),
                    Times.Never);

            this.validationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfUrlIsInvalidAndLogItAsync()
        {
            // given
            Source randomSource = CreateRandomSource();
            Source invalidSource = randomSource;
            string invalidUrl = GetRandomString();
            invalidSource.Url = invalidUrl;

            (dynamic Rule, string Parameter)[] validationCriteria =
            [
                (
                    Rule: new { Condition = false, Message = "Id is invalid" },
                    Parameter: nameof(Source.Id)
                ),

                (
                    Rule: new { Condition = false, Message = "Text is invalid" },
                    Parameter: nameof(Source.Name)
                ),

                (
                    Rule: new { Condition = true, Message = "Url is invalid" },
                    Parameter: nameof(Source.Url)
                ),

                (
                    Rule: new { Condition = false, Message = "Text exceed max length of 255 characters" },
                    Parameter: nameof(Source.Name)
                ),

                (
                    Rule: new { Condition = false, Message = "Date is invalid" },
                    Parameter: nameof(Source.CreatedDate)
                ),

                (
                    Rule: new { Condition = false, Message = "Text is invalid" },
                    Parameter: nameof(Source.CreatedBy)
                ),

                (
                    Rule: new { Condition = false, Message = "Date is invalid" },
                    Parameter: nameof(Source.UpdatedDate)
                ),

                (
                    Rule: new { Condition = false, Message = "Text is invalid" },
                    Parameter: nameof(Source.UpdatedBy)
                ),

                (
                    Rule: new { Condition = false, Message = $"Date is not the same as {nameof(Source.CreatedDate)}" },
                    Parameter: nameof(Source.UpdatedDate)
                ),

                (
                    Rule: new { Condition = false, Message = $"Text is not the same as {nameof(Source.CreatedBy)}" },
                    Parameter: nameof(Source.UpdatedBy)
                )
            ];

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.Url),
                values: "Url is invalid");

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: invalidSourceException);

            this.validationBrokerMock
                .Setup(broker =>
                    broker.Validate<InvalidSourceException>(
                        "Source is invalid, fix the errors and try again.",
                        It.IsAny<(dynamic Rule, string Parameter)[]>()))
                .ThrowsAsync(invalidSourceException);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.validationBrokerMock.Verify(broker =>
                broker.Validate<InvalidSourceException>(
                    "Source is invalid, fix the errors and try again.",
                    It.Is(Validations.Comparer.SameRulesAs(validationCriteria, output, "Validation Rules:"))),
                            Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedSourceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(It.IsAny<Source>()),
                    Times.Never);

            this.validationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAuditPropertiesIsNotTheSameAndLogItAsync()
        {
            // given
            Source randomSource = CreateRandomSource();
            Source invalidSource = randomSource;
            invalidSource.CreatedBy = GetRandomString();
            invalidSource.UpdatedBy = GetRandomString();
            invalidSource.CreatedDate = GetRandomDateTimeOffset();
            invalidSource.UpdatedDate = GetRandomDateTimeOffset();

            (dynamic Rule, string Parameter)[] validationCriteria =
            [
                (
                    Rule: new { Condition = false, Message = "Id is invalid" },
                    Parameter: nameof(Source.Id)
                ),

                (
                    Rule: new { Condition = false, Message = "Text is invalid" },
                    Parameter: nameof(Source.Name)
                ),

                (
                    Rule: new { Condition = false, Message = "Text exceed max length of 255 characters" },
                    Parameter: nameof(Source.Name)
                ),

                (
                    Rule: new { Condition = false, Message = "Text is invalid" },
                    Parameter: nameof(Source.CreatedBy)
                ),

                (
                    Rule: new { Condition = false, Message = "Text is invalid" },
                    Parameter: nameof(Source.UpdatedBy)
                ),

                (
                    Rule: new { Condition = false, Message = "Date is invalid" },
                    Parameter: nameof(Source.CreatedDate)
                ),

                (
                    Rule: new { Condition = false, Message = "Date is invalid" },
                    Parameter: nameof(Source.UpdatedDate)
                ),

                (
                    Rule: new { Condition = false, Message = "Url is invalid" },
                    Parameter: nameof(Source.Url)
                ),

                (
                    Rule: new { Condition = true, Message = $"Text is not the same as {nameof(Source.CreatedBy)}" },
                    Parameter: nameof(Source.UpdatedBy)
                ),

                (
                    Rule: new { Condition = true, Message = $"Date is not the same as {nameof(Source.CreatedDate)}" },
                    Parameter: nameof(Source.UpdatedDate)
                )
            ];

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

            this.validationBrokerMock
                .Setup(broker =>
                    broker.Validate<InvalidSourceException>(
                        "Source is invalid, fix the errors and try again.",
                        It.IsAny<(dynamic Rule, string Parameter)[]>()))
                .ThrowsAsync(invalidSourceException);

            // when
            ValueTask<Source> addSourceTask =
                this.sourceService.AddSourceAsync(invalidSource);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    addSourceTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.validationBrokerMock.Verify(broker =>
                broker.Validate<InvalidSourceException>(
                    "Source is invalid, fix the errors and try again.",
                    It.Is(Validations.Comparer.SameRulesAs(validationCriteria, output, "Validation Rules:"))),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedSourceValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSourceAsync(It.IsAny<Source>()),
                    Times.Never);

            this.validationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
