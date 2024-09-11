// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Source someSource = CreateRandomSource();
            SqlException sqlException = CreateSqlException();

            var failedSourceStorageException =
                new FailedStorageSourceException(
                    message: "Failed source storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedSourceDependencyException =
                new SourceDependencyException(
                    message: "Source dependency error occurred, contact support.",
                        innerException: failedSourceStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Source> modifySourceTask =
                this.sourceService.ModifySourceAsync(someSource);

            SourceDependencyException actualSourceDependencyException =
                await Assert.ThrowsAsync<SourceDependencyException>(
                    modifySourceTask.AsTask);

            // then
            actualSourceDependencyException.Should().BeEquivalentTo(
                expectedSourceDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(someSource.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedSourceDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSourceAsync(someSource),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Source randomSource =
                CreateRandomSource(randomDateTimeOffset);

            randomSource.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var dbUpdateException = new DbUpdateException();

            var failedOperationSourceException =
                new FailedOperationSourceException(
                    message: "Failed operation source  error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedSourceDependencyException =
                new SourceDependencyException(
                    message: "Source dependency error occurred, contact support.",
                    innerException: failedOperationSourceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(randomSource.Id))
                    .ThrowsAsync(dbUpdateException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Source> modifySourceTask =
                this.sourceService.ModifySourceAsync(randomSource);

            SourceDependencyException actualSourceDependencyException =
                await Assert.ThrowsAsync<SourceDependencyException>(
                    modifySourceTask.AsTask);

            // then
            actualSourceDependencyException.Should().BeEquivalentTo(
                expectedSourceDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(randomSource.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSourceDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        private async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Source randomSource = CreateRandomSource(randomDateTimeOffset);

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedSourceException =
                new LockedSourceException(
                    message: "Locked source record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException);

            var expectedSourceDependencyValidationException =
                new SourceDependencyValidationException(
                    message: "Source dependency validation error occurred, fix errors and try again.",
                    innerException: lockedSourceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Source> modifySourceTask =
                this.sourceService.ModifySourceAsync(randomSource);

            SourceDependencyValidationException actualSourceDependencyValidationException =
                await Assert.ThrowsAsync<SourceDependencyValidationException>(
                    modifySourceTask.AsTask);

            // then
            actualSourceDependencyValidationException.Should().BeEquivalentTo(
                expectedSourceDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSourceDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(randomSource.Id),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Source randomSource =
                CreateRandomSource(randomDateTimeOffset);

            randomSource.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var serviceException = new Exception();

            var failedServiceSourceException =
                new FailedServiceSourceException(
                    message: "Failed service source error occurred, contact support.",
                    innerException: serviceException);

            var expectedSourceServiceException =
                new SourceServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceSourceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(randomSource.Id))
                    .ThrowsAsync(serviceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Source> modifySourceTask =
                this.sourceService.ModifySourceAsync(randomSource);

            SourceServiceException actualSourceServiceException =
                await Assert.ThrowsAsync<SourceServiceException>(
                    modifySourceTask.AsTask);

            // then
            actualSourceServiceException.Should().BeEquivalentTo(
                expectedSourceServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(randomSource.Id),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedSourceServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}