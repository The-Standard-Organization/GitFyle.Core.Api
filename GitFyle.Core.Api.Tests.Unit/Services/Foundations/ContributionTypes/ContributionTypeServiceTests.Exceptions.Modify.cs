// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Moq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ContributionType someContributionType = CreateRandomContributionType();
            SqlException sqlException = CreateSqlException();

            var failedContributionTypeStorageException =
                new FailedStorageContributionTypeException(
                    message: "Failed storage contributionType error occurred, contact support.",
                        innerException: sqlException);

            var expectedContributionTypeDependencyException =
                new ContributionTypeDependencyException(
                    message: "ContributionType dependency error occurred, contact support.",
                        innerException: failedContributionTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ContributionType> modifyContributionTypeTask =
                this.contributionTypeService.ModifyContributionTypeAsync(someContributionType);

            ContributionTypeDependencyException actualContributionTypeDependencyException =
                await Assert.ThrowsAsync<ContributionTypeDependencyException>(
                    testCode: modifyContributionTypeTask.AsTask);

            // then
            actualContributionTypeDependencyException.Should().BeEquivalentTo(
                expectedContributionTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(someContributionType.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributionTypeAsync(someContributionType),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            ContributionType foreignKeyConflictedContributionType = CreateRandomContributionType();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(message: exceptionMessage);

            var invalidContributionTypeReferenceException =
                new InvalidReferenceContributionTypeException(
                    message: "Invalid contributionType reference error occurred.", 
                    innerException: foreignKeyConstraintConflictException,
                    data: foreignKeyConstraintConflictException.Data);

            var expectedContributionTypeDependencyValidationException =
            new ContributionTypeDependencyValidationException(
                    message: "ContributionType dependency validation error occurred, fix errors and try again.", 
                    innerException: invalidContributionTypeReferenceException,
                    data: invalidContributionTypeReferenceException.Data);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<ContributionType> modifyContributionTypeTask =
                this.contributionTypeService.ModifyContributionTypeAsync(foreignKeyConflictedContributionType);

            ContributionTypeDependencyValidationException actualContributionTypeDependencyValidationException =
                await Assert.ThrowsAsync<ContributionTypeDependencyValidationException>(
                    modifyContributionTypeTask.AsTask);

            // then
            actualContributionTypeDependencyValidationException.Should().BeEquivalentTo(
                expectedContributionTypeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(foreignKeyConflictedContributionType.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateContributionTypeAsync(foreignKeyConflictedContributionType),
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

            ContributionType randomContributionType =
                CreateRandomContributionType(randomDateTimeOffset);

            randomContributionType.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var dbUpdateException = new DbUpdateException();

            var failedOperationContributionTypeException =
                new FailedOperationContributionTypeException(
                    message: "Failed operation contributionType error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedContributionTypeDependencyException =
                new ContributionTypeDependencyException(
                    message: "ContributionType dependency error occurred, contact support.",
                    innerException: failedOperationContributionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionTypeByIdAsync(randomContributionType.Id))
                    .ThrowsAsync(dbUpdateException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ContributionType> modifyContributionTypeTask =
                this.contributionTypeService.ModifyContributionTypeAsync(randomContributionType);

            ContributionTypeDependencyException actualContributionTypeDependencyException =
                await Assert.ThrowsAsync<ContributionTypeDependencyException>(
                    testCode: modifyContributionTypeTask.AsTask);

            // then
            actualContributionTypeDependencyException.Should().BeEquivalentTo(
                expectedContributionTypeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(randomContributionType.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeDependencyException))),
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
            ContributionType randomContributionType = CreateRandomContributionType(randomDateTimeOffset);

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedContributionTypeException =
                new LockedContributionTypeException(
                    message: "Locked contributionType record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedContributionTypeDependencyValidationException =
                new ContributionTypeDependencyValidationException(
                    message: "ContributionType dependency validation error occurred, fix errors and try again.",
                    innerException: lockedContributionTypeException,
                    data: lockedContributionTypeException.Data);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<ContributionType> modifyContributionTypeTask =
                this.contributionTypeService.ModifyContributionTypeAsync(randomContributionType);

            ContributionTypeDependencyValidationException actualContributionTypeDependencyValidationException =
                await Assert.ThrowsAsync<ContributionTypeDependencyValidationException>(
                    testCode: modifyContributionTypeTask.AsTask);

            // then
            actualContributionTypeDependencyValidationException.Should().BeEquivalentTo(
                expectedContributionTypeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(randomContributionType.Id),
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

            ContributionType randomContributionType =
                CreateRandomContributionType(randomDateTimeOffset);

            randomContributionType.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

            var serviceException = new Exception();

            var failedServiceContributionTypeException =
                new FailedServiceContributionTypeException(
                    message: "Failed service contributionType error occurred, contact support.",
                    innerException: serviceException);

            var expectedContributionTypeServiceException =
                new ContributionTypeServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributionTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectContributionTypeByIdAsync(randomContributionType.Id))
                    .ThrowsAsync(serviceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ContributionType> modifyContributionTypeTask =
                this.contributionTypeService.ModifyContributionTypeAsync(randomContributionType);

            ContributionTypeServiceException actualContributionTypeServiceException =
                await Assert.ThrowsAsync<ContributionTypeServiceException>(
                    testCode: modifyContributionTypeTask.AsTask);

            // then
            actualContributionTypeServiceException.Should().BeEquivalentTo(
                expectedContributionTypeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectContributionTypeByIdAsync(randomContributionType.Id),
                    Times.Once());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}