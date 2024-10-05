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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Contribution someContribution = CreateRandomContribution();
            SqlException sqlException = CreateSqlException();

            var failedContributionStorageException =
                new FailedStorageContributionException(
                    message: "Failed contribution storage error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributionDependencyException =
                new ContributionDependencyException(
                    message: "Contribution dependency error occurred, contact support.",
                        innerException: failedContributionStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(someContribution);

            ContributionDependencyException actualContributionDependencyException =
                await Assert.ThrowsAsync<ContributionDependencyException>(
                    modifyContributionTask.AsTask);

            // then
            actualContributionDependencyException.Should().BeEquivalentTo(
                expectedContributionDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(someContribution.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributionAsync(someContribution),
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

            Contribution randomContribution =
                CreateRandomContribution(randomDateTimeOffset);

            randomContribution.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var dbUpdateException = new DbUpdateException();

            var failedOperationContributionException =
                new FailedOperationContributionException(
                    message: "Failed operation contribution  error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedContributionDependencyException =
                new ContributionDependencyException(
                    message: "Contribution dependency error occurred, contact support.",
                    innerException: failedOperationContributionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(randomContribution.Id))
                    .ThrowsAsync(dbUpdateException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(randomContribution);

            ContributionDependencyException actualContributionDependencyException =
                await Assert.ThrowsAsync<ContributionDependencyException>(
                    modifyContributionTask.AsTask);

            // then
            actualContributionDependencyException.Should().BeEquivalentTo(
                expectedContributionDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(randomContribution.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionDependencyException))),
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
            Contribution randomContribution = CreateRandomContribution(randomDateTimeOffset);

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedContributionException =
                new LockedContributionException(
                    message: "Locked contribution record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException);

            var expectedContributionDependencyValidationException =
                new ContributionDependencyValidationException(
                    message: "Contribution dependency validation error occurred, fix errors and try again.",
                    innerException: lockedContributionException, 
                    data: lockedContributionException.Data);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(randomContribution);

            ContributionDependencyValidationException actualContributionDependencyValidationException =
                await Assert.ThrowsAsync<ContributionDependencyValidationException>(
                    modifyContributionTask.AsTask);

            // then
            actualContributionDependencyValidationException.Should().BeEquivalentTo(
                expectedContributionDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(randomContribution.Id),
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

            Contribution randomContribution =
                CreateRandomContribution(randomDateTimeOffset);

            randomContribution.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var serviceException = new Exception();

            var failedServiceContributionException =
                new FailedServiceContributionException(
                    message: "Failed service contribution error occurred, contact support.",
                    innerException: serviceException);

            var expectedContributionServiceException =
                new ContributionServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionByIdAsync(randomContribution.Id))
                    .ThrowsAsync(serviceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contribution> modifyContributionTask =
                this.contributionService.ModifyContributionAsync(randomContribution);

            ContributionServiceException actualContributionServiceException =
                await Assert.ThrowsAsync<ContributionServiceException>(
                    modifyContributionTask.AsTask);

            // then
            actualContributionServiceException.Should().BeEquivalentTo(
                expectedContributionServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionByIdAsync(randomContribution.Id),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}