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
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.ContributionTypes
{
    public partial class ContributionTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccurredAndLogItAsync()
        {
            // given
            ContributionType someContributionType = CreateRandomContributionType();
            SqlException sqlException = CreateSqlException();

            var failedStorageContributionTypeException =
                new FailedStorageContributionTypeException(
                    message: "Failed storage contributionType error occurred, contact support.",
                    innerException: sqlException);

            var expectedContributionTypeDependencyException =
                new ContributionTypeDependencyException(
                    message: "ContributionType dependency error occurred, contact support.",
                    innerException: failedStorageContributionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionTypeService.AddContributionTypeAsync(
                    someContributionType);

            ContributionTypeDependencyException actualContributionTypeDependencyException =
                await Assert.ThrowsAsync<ContributionTypeDependencyException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeDependencyException.Should().BeEquivalentTo(
                expectedContributionTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
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
            ValueTask<ContributionType> AddContributionTypeTask =
                this.contributionTypeService.AddContributionTypeAsync(foreignKeyConflictedContributionType);

            ContributionTypeDependencyValidationException actualContributionTypeDependencyValidationException =
                await Assert.ThrowsAsync<ContributionTypeDependencyValidationException>(
                    AddContributionTypeTask.AsTask);

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
                broker.InsertContributionTypeAsync(foreignKeyConflictedContributionType),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfContributionTypeAlreadyExistsAndLogItAsync()
        {
            // given
            ContributionType someContributionType = CreateRandomContributionType();

            var duplicateKeyException =
                new DuplicateKeyException(
                    message: "Duplicate key error occurred");

            var alreadyExistsContributionTypeException =
                new AlreadyExistsContributionTypeException(
                    message: "ContributionType already exists error occurred.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedContributionTypeDependencyValidationException =
                new ContributionTypeDependencyValidationException(
                    message: "ContributionType dependency validation error occurred, fix errors and try again.",
                    innerException: alreadyExistsContributionTypeException,
                    data: alreadyExistsContributionTypeException.Data);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionTypeService.AddContributionTypeAsync(
                    someContributionType);

            ContributionTypeDependencyValidationException actualContributionTypeDependencyValidationException =
                await Assert.ThrowsAsync<ContributionTypeDependencyValidationException>(
                    testCode: addContributionTypeTask.AsTask);

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
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyErrorOccurredAndLogItAsync()
        {
            // given
            ContributionType someContributionType = CreateRandomContributionType();
            var dbUpdateException = new DbUpdateException();

            var failedOperationContributionTypeException =
                new FailedOperationContributionTypeException(
                    message: "Failed operation contributionType error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedContributionTypeDependencyException =
                new ContributionTypeDependencyException(
                    message: "ContributionType dependency error occurred, contact support.",
                    innerException: failedOperationContributionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionTypeService.AddContributionTypeAsync(
                    someContributionType);

            ContributionTypeDependencyException actualContributionTypeDependencyException =
                await Assert.ThrowsAsync<ContributionTypeDependencyException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeDependencyException.Should().BeEquivalentTo(
                expectedContributionTypeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            ContributionType randomContributionType = CreateRandomContributionType();
            var serviceException = new Exception();

            var failedServiceContributionTypeException =
                new FailedServiceContributionTypeException(
                    message: "Failed service contributionType error occurred, contact support.",
                    innerException: serviceException);

            var expectedContributionTypeServiceException =
                new ContributionTypeServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceContributionTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ContributionType> addContributionTypeTask =
                this.contributionTypeService.AddContributionTypeAsync(
                    randomContributionType);

            ContributionTypeServiceException actualContributionTypeServiceException =
                await Assert.ThrowsAsync<ContributionTypeServiceException>(
                    testCode: addContributionTypeTask.AsTask);

            // then
            actualContributionTypeServiceException.Should().BeEquivalentTo(
                expectedContributionTypeServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedContributionTypeServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertContributionTypeAsync(It.IsAny<ContributionType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}