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
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\t \n\r")]
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

            (dynamic Rule, string Parameter)[] validationCriteria = CreateValidationCriteria(
                    source: invalidSource,
                    idCondition: true,
                    nameCondition: true,
                    urlCondition: true,
                    createdByCondition: true,
                    updatedByCondition: true,
                    createdDateCondition: true,
                    updatedDateCondition: true);

            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

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
                        It.Is(Validations.Comparer.SameRulesAs(validationCriteria))),
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
                CreateValidationCriteria(source: invalidSource, urlCondition: true);

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

            (dynamic Rule, string Parameter)[] validationCriteria = CreateValidationCriteria(
                    source: invalidSource,
                    updatedByMatchCreatedByCondition: true,
                    updatedDateMatchCreatedDateCondition: true);

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
