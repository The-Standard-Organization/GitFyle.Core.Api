// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someContributionGuid = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedContributionStorageException =
                new FailedStorageContributionException(
                    message: "Failed contribution storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributionDependencyException =
                new ContributionDependencyException(
                    message: "Contribution dependency error occurred, contact support.",
                        innerException: failedContributionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(someContributionGuid))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contribution> removeContributionByIdTask =
                this.contributionService.RemoveContributionByIdAsync(someContributionGuid);

            ContributionDependencyException actualContributionDependencyException =
                await Assert.ThrowsAsync<ContributionDependencyException>(
                    removeContributionByIdTask.AsTask);

            // then
            actualContributionDependencyException.Should().BeEquivalentTo(
                expectedContributionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributionDependencyException))),
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
            Guid someContributionId = Guid.NewGuid();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedContributionException =
                new LockedContributionException(
                    message: "Locked contribution record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedContributionDependencyValidationException =
                new ContributionDependencyValidationException(
                    message: "Contribution dependency validation error occurred, fix errors and try again.",
                    innerException: lockedContributionException,
                    data: lockedContributionException.Data);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(someContributionId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Contribution> removeContributionByIdTask =
                this.contributionService.RemoveContributionByIdAsync(someContributionId);

            ContributionDependencyValidationException actualContributionDependencyValidationException =
                await Assert.ThrowsAsync<ContributionDependencyValidationException>(
                    removeContributionByIdTask.AsTask);

            // then
            actualContributionDependencyValidationException.Should().BeEquivalentTo(
                expectedContributionDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionDependencyValidationException))),
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
            Guid someContributionId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedContributionServiceException =
                new FailedServiceContributionException(
                    message: "Failed service contribution error occurred, contact support.",
                    innerException: serviceException);

            var expectedContributionServiceException =
                new ContributionServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedContributionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Contribution> removeContributionByIdTask =
                this.contributionService.RemoveContributionByIdAsync(someContributionId);

            ContributionServiceException actualContributionServiceException =
                await Assert.ThrowsAsync<ContributionServiceException>(
                    removeContributionByIdTask.AsTask);

            // then
            actualContributionServiceException.Should()
                .BeEquivalentTo(expectedContributionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}