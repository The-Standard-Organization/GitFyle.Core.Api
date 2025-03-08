// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Repositories
{
    public partial class RepositoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someRepositoryGuid = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedRepositoryStorageException =
                new FailedStorageRepositoryException(
                    message: "Failed storage repository error occurred, contact support.",
                        innerException: sqlException);

            var expectedRepositoryDependencyException =
                new RepositoryDependencyException(
                    message: "Repository dependency error occurred, contact support.",
                        innerException: failedRepositoryStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(someRepositoryGuid))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Repository> removeRepositoryByIdTask =
                this.repositoryService.RemoveRepositoryByIdAsync(someRepositoryGuid);

            RepositoryDependencyException actualRepositoryDependencyException =
                await Assert.ThrowsAsync<RepositoryDependencyException>(
                    removeRepositoryByIdTask.AsTask);

            // then
            actualRepositoryDependencyException.Should().BeEquivalentTo(
                expectedRepositoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(It.IsAny<Guid>()),
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
        private async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDbConcurrencyOccursAndLogItAsync()
        {
            // given
            Guid someRepositoryId = Guid.NewGuid();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedRepositoryException =
                new LockedRepositoryException(
                    message: "Locked repository record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedRepositoryDependencyValidationException =
                new RepositoryDependencyValidationException(
                    message: "Repository validation error occurred, fix errors and try again.",
                    innerException: lockedRepositoryException,
                    data: lockedRepositoryException.Data);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(someRepositoryId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Repository> removeRepositoryByIdTask =
                this.repositoryService.RemoveRepositoryByIdAsync(someRepositoryId);

            RepositoryDependencyValidationException actualRepositoryDependencyValidationException =
                await Assert.ThrowsAsync<RepositoryDependencyValidationException>(
                    removeRepositoryByIdTask.AsTask);

            // then
            actualRepositoryDependencyValidationException.Should().BeEquivalentTo(
                expectedRepositoryDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRepositoryDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someRepositoryId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedRepositoryServiceException =
                new FailedServiceRepositoryException(
                    message: "Failed service repository error occurred, contact support.",
                    innerException: serviceException);

            var expectedRepositoryServiceException =
                new RepositoryServiceException(
                    message: "Repository service error occurred, contact support.",
                    innerException: failedRepositoryServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Repository> removeRepositoryByIdTask =
                this.repositoryService.RemoveRepositoryByIdAsync(someRepositoryId);

            RepositoryServiceException actualRepositoryServiceException =
                await Assert.ThrowsAsync<RepositoryServiceException>(
                    removeRepositoryByIdTask.AsTask);

            // then
            actualRepositoryServiceException.Should()
                .BeEquivalentTo(expectedRepositoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectRepositoryByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRepositoryServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}