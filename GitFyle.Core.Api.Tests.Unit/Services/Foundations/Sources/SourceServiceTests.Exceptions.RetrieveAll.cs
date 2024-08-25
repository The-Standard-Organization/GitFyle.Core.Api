// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageSourceException =
                new FailedStorageSourceException(
                    message: "Failed source storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedSourceDependencyException =
                new SourceDependencyException(
                    message: "Source dependency error occurred, contact support.",
                    innerException: failedStorageSourceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSourcesAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Source>> retrieveAllSourcesTask =
                this.sourceService.RetrieveAllSourcesAsync();

            SourceDependencyException actualSourceDependencyException =
                await Assert.ThrowsAsync<SourceDependencyException>(
                    testCode: retrieveAllSourcesTask.AsTask);

            // then
            actualSourceDependencyException.Should().BeEquivalentTo(
                expectedSourceDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSourcesAsync(),
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
        public async Task ShouldThrowServiceErrorOnRetrieveAllWhenServiceErrorOccursAndLogItAsync()
        {
            // given
            var serviceError = new Exception();

            var failedServiceSourceException =
                new FailedServiceSourceException(
                    message: "Failed service source error occurred, contact support.",
                    innerException: serviceError);

            var expectedSourceServiceException =
                new SourceServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceSourceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSourcesAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<Source>> retrieveAllSourcesTask =
                this.sourceService.RetrieveAllSourcesAsync();

            SourceServiceException actualSourceServiceException =
                await Assert.ThrowsAsync<SourceServiceException>(
                    testCode: retrieveAllSourcesTask.AsTask);

            // then
            actualSourceServiceException.Should().BeEquivalentTo(
                expectedSourceServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSourcesAsync(),
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