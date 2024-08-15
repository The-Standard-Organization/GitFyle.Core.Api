// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Moq;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdWhenSourceIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidSourceId = Guid.Empty;

            var invalidSourceException =
                new InvalidSourceException(
                    message: "Source is invalid, fix the errors and try again.");

            invalidSourceException.AddData(
                key: nameof(Source.Id),
                values: "Id is invalid");

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: invalidSourceException);

            // when
            ValueTask<Source> retrieveSourceByIdTask =
                this.sourceService.RetrieveSourceByIdAsync(invalidSourceId);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    retrieveSourceByIdTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSourceValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfSourceIdNotFoundAndLogitAsync()
        {
            //given
            var someSourceId = Guid.NewGuid();
            Source nullSource = null;
            var innerException = new Exception();

            var notFoundSourceException =
                new NotFoundSourceException(
                    message: $"Source not found with id: {someSourceId}",
                    innerException: innerException.InnerException.As<Xeption>());

            var expectedSourceValidationException =
                new SourceValidationException(
                    message: "Source validation error occurred, fix errors and try again.",
                    innerException: notFoundSourceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(someSourceId))
                    .ReturnsAsync(nullSource);
        
            // when
            ValueTask<Source> retrieveSourceByIdTask =
                this.sourceService.RetrieveSourceByIdAsync(someSourceId);

            SourceValidationException actualSourceValidationException =
                await Assert.ThrowsAsync<SourceValidationException>(
                    retrieveSourceByIdTask.AsTask);

            // then
            actualSourceValidationException.Should().BeEquivalentTo(
                expectedSourceValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(someSourceId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSourceValidationException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}