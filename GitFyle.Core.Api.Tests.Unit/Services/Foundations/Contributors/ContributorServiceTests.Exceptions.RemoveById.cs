// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someContributorGuid = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedContributorStorageException =
                new FailedStorageContributorException(
                    message: "Failed storage contributor error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributorDependencyException =
                new ContributorDependencyException(
                    message: "Contributor dependency error occurred, contact support.",
                        innerException: failedContributorStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(someContributorGuid))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contributor> removeContributorByIdTask =
                this.contributorService.RemoveContributorByIdAsync(someContributorGuid);

            ContributorDependencyException actualContributorDependencyException =
                await Assert.ThrowsAsync<ContributorDependencyException>(
                    removeContributorByIdTask.AsTask);

            // then
            actualContributorDependencyException.Should().BeEquivalentTo(
                expectedContributorDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributorDependencyException))),
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
            Guid someContributorId = Guid.NewGuid();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedContributorException =
                new LockedContributorException(
                    message: "Locked contributor record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedContributorDependencyValidationException =
                new ContributorDependencyValidationException(
                    message: "Contributor dependency validation error occurred, fix errors and try again.",
                    innerException: lockedContributorException,
                    data: lockedContributorException.Data);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(someContributorId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Contributor> removeContributorByIdTask =
                this.contributorService.RemoveContributorByIdAsync(someContributorId);

            ContributorDependencyValidationException actualContributorDependencyValidationException =
                await Assert.ThrowsAsync<ContributorDependencyValidationException>(
                    removeContributorByIdTask.AsTask);

            // then
            actualContributorDependencyValidationException.Should().BeEquivalentTo(
                expectedContributorDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorDependencyValidationException))),
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