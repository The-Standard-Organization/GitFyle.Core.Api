// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Repositories
{
    public partial class RepositoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSQLExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();

            var failedStorageRepositoryException =
                new FailedStorageRepositoryException(
                    message: "Failed repository storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedRepositoryDependencyException =
                new RepositoryDependencyException(
                    message: "Repository dependency error occurred, contact support.",
                    innerException: failedStorageRepositoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllRepositoriesAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Repository>> retrieveAllRepositoriesTask =
                this.repositoryService.RetrieveAllRepositoriesAsync();

            RepositoryDependencyException actualRepositoryDependencyException =
                await Assert.ThrowsAsync<RepositoryDependencyException>(
                    testCode: retrieveAllRepositoriesTask.AsTask);

            // then
            actualRepositoryDependencyException.Should().BeEquivalentTo(
                expectedRepositoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllRepositoriesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedRepositoryDependencyException))),
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

            var failedServiceRepositoryException =
                new FailedServiceRepositoryException(
                    message: "Failed service repository error occurred, contact support.",
                    innerException: serviceError);

            var expectedRepositoryServiceException =
                new RepositoryServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceRepositoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllRepositoriesAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<Repository>> retrieveAllRepositoriesTask =
                this.repositoryService.RetrieveAllRepositoriesAsync();

            RepositoryServiceException actualRepositoryServiceException =
                await Assert.ThrowsAsync<RepositoryServiceException>(
                    testCode: retrieveAllRepositoriesTask.AsTask);

            // then
            actualRepositoryServiceException.Should().BeEquivalentTo(
                expectedRepositoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllRepositoriesAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRepositoryServiceException))),
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