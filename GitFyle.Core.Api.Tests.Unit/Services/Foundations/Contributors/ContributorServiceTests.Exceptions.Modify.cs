// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Contributor someContributor = CreateRandomContributor();
            SqlException sqlException = CreateSqlException();

            var failedContributorStorageException =
                new FailedStorageContributorException(
                    message: "Failed storage contributor error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributorDependencyException =
                new ContributorDependencyException(
                    message: "Contributor dependency error occurred, contact support.",
                        innerException: failedContributorStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(someContributor);

            ContributorDependencyException actualContributorDependencyException =
                await Assert.ThrowsAsync<ContributorDependencyException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorDependencyException.Should().BeEquivalentTo(
                expectedContributorDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(someContributor.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributorDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributorAsync(someContributor),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Contributor foreignKeyConflictedContributor = CreateRandomContributor();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(message: exceptionMessage);

            var invalidContributorReferenceException =
                new InvalidReferenceContributorException(
                    message: "Invalid contributor reference error occurred.",
                    innerException: foreignKeyConstraintConflictException,
                    data: foreignKeyConstraintConflictException.Data);

            var expectedContributorDependencyValidationException =
                new ContributorDependencyValidationException(
                    message: "Contributor dependency validation error occurred, fix errors and try again.",
                    innerException: invalidContributorReferenceException,
                    data: invalidContributorReferenceException.Data);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(foreignKeyConflictedContributor);

            ContributorDependencyValidationException actualContributorDependencyValidationException =
                await Assert.ThrowsAsync<ContributorDependencyValidationException>(
                    modifyContributorTask.AsTask);

            // then
            actualContributorDependencyValidationException.Should().BeEquivalentTo(
                expectedContributorDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(foreignKeyConflictedContributor.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributorAsync(foreignKeyConflictedContributor),
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

            Contributor randomContributor =
                CreateRandomContributor(randomDateTimeOffset);

            randomContributor.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var dbUpdateException = new DbUpdateException();

            var failedOperationContributorException =
                new FailedOperationContributorException(
                    message: "Failed operation contributor error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedContributorDependencyException =
                new ContributorDependencyException(
                    message: "Contributor dependency error occurred, contact support.",
                    innerException: failedOperationContributorException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(randomContributor.Id))
                    .ThrowsAsync(dbUpdateException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(randomContributor);

            ContributorDependencyException actualContributorDependencyException =
                await Assert.ThrowsAsync<ContributorDependencyException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorDependencyException.Should().BeEquivalentTo(
                expectedContributorDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(randomContributor.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorDependencyException))),
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
            Contributor randomContributor = CreateRandomContributor(randomDateTimeOffset);

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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(randomContributor);

            ContributorDependencyValidationException actualContributorDependencyValidationException =
                await Assert.ThrowsAsync<ContributorDependencyValidationException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorDependencyValidationException.Should().BeEquivalentTo(
                expectedContributorDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(randomContributor.Id),
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

            Contributor randomContributor =
                CreateRandomContributor(randomDateTimeOffset);

            randomContributor.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var serviceException = new Exception();

            var failedServiceContributorException =
                new FailedServiceContributorException(
                    message: "Failed service contributor error occurred, contact support.",
                    innerException: serviceException);

            var expectedContributorServiceException =
                new ContributorServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributorException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributorByIdAsync(randomContributor.Id))
                    .ThrowsAsync(serviceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<Contributor> modifyContributorTask =
                this.contributorService.ModifyContributorAsync(randomContributor);

            ContributorServiceException actualContributorServiceException =
                await Assert.ThrowsAsync<ContributorServiceException>(
                    testCode: modifyContributorTask.AsTask);

            // then
            actualContributorServiceException.Should().BeEquivalentTo(
                expectedContributorServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributorByIdAsync(randomContributor.Id),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributorServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}