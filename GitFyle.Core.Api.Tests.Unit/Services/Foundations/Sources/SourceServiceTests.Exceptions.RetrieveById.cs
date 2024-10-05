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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSQLErrorOccursAndLogItAsync()
        {
            // given
            var someSourceId = Guid.NewGuid();
            var sqlException = CreateSqlException();

            var failedStorageSourceException =
                new FailedStorageSourceException(
                    message: "Failed source storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedSourceDependencyException =
                new SourceDependencyException(
                    message: "Source dependency error occurred, contact support.",
                    innerException: failedStorageSourceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSourceByIdAsync(someSourceId))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<Source> retrieveSourceByIdTask =
                this.sourceService.RetrieveSourceByIdAsync(someSourceId);

            // then
            await Assert.ThrowsAsync<SourceDependencyException>(
                testCode: retrieveSourceByIdTask.AsTask);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(someSourceId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedSourceDependencyException))),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            //given
            var someSourceId = Guid.NewGuid();
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
                broker.SelectSourceByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            //when
            ValueTask<Source> retrieveSourceByIdTask =
                this.sourceService.RetrieveSourceByIdAsync(someSourceId);

            SourceServiceException actualSourceServiceException =
                await Assert.ThrowsAsync<SourceServiceException>(
                    testCode: retrieveSourceByIdTask.AsTask);

            //then
            actualSourceServiceException.Should().BeEquivalentTo(
                expectedSourceServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSourceByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogErrorAsync(It.Is(SameExceptionAs(
                expectedSourceServiceException))),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}