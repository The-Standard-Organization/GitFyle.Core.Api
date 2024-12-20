﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccurredAndLogItAsync()
        {
            // given
            Contribution someContribution = CreateRandomContribution();
            SqlException sqlException = CreateSqlException();

            var failedStorageContributionException =
                new FailedStorageContributionException(
                    message: "Failed contribution storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedContributionDependencyException =
                new ContributionDependencyException(
                    message: "Contribution dependency error occurred, contact support.",
                    innerException: failedStorageContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contribution> addContributionTask =
                this.contributionService.AddContributionAsync(
                    someContribution);

            ContributionDependencyException actualContributionDependencyException =
                await Assert.ThrowsAsync<ContributionDependencyException>(
                    testCode: addContributionTask.AsTask);

            // then
            actualContributionDependencyException.Should().BeEquivalentTo(
                expectedContributionDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionAsync(It.IsAny<Contribution>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfContributionAlreadyExistsAndLogItAsync()
        {
            // given
            Contribution someContribution = CreateRandomContribution();

            var duplicateKeyException =
                new DuplicateKeyException(
                    message: "Duplicate key error occurred");

            var alreadyExistsContributionException =
                new AlreadyExistsContributionException(
                    message: "Contribution already exists error occurred.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedContributionDependencyValidationException =
                new ContributionDependencyValidationException(
                    message: "Contribution dependency validation error occurred, fix errors and try again.",
                    innerException: alreadyExistsContributionException,
                    data: alreadyExistsContributionException.Data);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Contribution> addContributionTask =
                this.contributionService.AddContributionAsync(
                    someContribution);

            ContributionDependencyValidationException actualContributionDependencyValidationException =
                await Assert.ThrowsAsync<ContributionDependencyValidationException>(
                    testCode: addContributionTask.AsTask);

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
                broker.InsertContributionAsync(It.IsAny<Contribution>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyErrorOccurredAndLogItAsync()
        {
            // given
            Contribution someContribution = CreateRandomContribution();
            var dbUpdateException = new DbUpdateException();

            var failedOperationContributionException =
                new FailedOperationContributionException(
                    message: "Failed operation contribution  error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedContributionDependencyException =
                new ContributionDependencyException(
                    message: "Contribution dependency error occurred, contact support.",
                    innerException: failedOperationContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<Contribution> addContributionTask =
                this.contributionService.AddContributionAsync(
                    someContribution);

            ContributionDependencyException actualContributionDependencyException =
                await Assert.ThrowsAsync<ContributionDependencyException>(
                    testCode: addContributionTask.AsTask);

            // then
            actualContributionDependencyException.Should().BeEquivalentTo(
                expectedContributionDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionAsync(It.IsAny<Contribution>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            Contribution randomContribution = CreateRandomContribution();
            var serviceException = new Exception();

            var failedServiceContributionException =
                new FailedServiceContributionException(
                    message: "Failed service contribution error occurred, contact support.",
                    innerException: serviceException);

            var expectedContributionServiceException =
                new ContributionServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Contribution> addContributionTask =
                this.contributionService.AddContributionAsync(
                    randomContribution);

            ContributionServiceException actualContributionServiceException =
                await Assert.ThrowsAsync<ContributionServiceException>(
                    testCode: addContributionTask.AsTask);

            // then
            actualContributionServiceException.Should().BeEquivalentTo(
                expectedContributionServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionAsync(It.IsAny<Contribution>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}