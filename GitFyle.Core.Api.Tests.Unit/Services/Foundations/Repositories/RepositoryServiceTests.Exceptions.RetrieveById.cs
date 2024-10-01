// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Repositories
{
    public partial class RepositoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSQLErrorOccursAndLogItAsync()
        {
            // given
            Guid someGuid = Guid.NewGuid();
            var sqlException = CreateSqlException();

            var failedStorageRepositoryException =
                new FailedStorageRepositoryException(
                    message: "Failed storage repository error occurred, contact support.",
                    innerException: sqlException);

            var expectedRepositoryDependencyException =
                new RepositoryDependencyException(
                    message: "Repository dependency error occurred, contact support.",
                    innerException: failedStorageRepositoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectRepositoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Repository> retrieveRepositoryByIdTask =
                this.repositoryService.RetrieveRepositoryByIdAsync(someGuid);

            // then
            await Assert.ThrowsAsync<RepositoryDependencyException>(
                retrieveRepositoryByIdTask.AsTask);

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
    }
}