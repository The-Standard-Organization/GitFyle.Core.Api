// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Configuration someConfiguration =
                CreateRandomConfiguration(DateTimeOffset.UtcNow);

            SqlException sqlException = CreateSqlException();

            var failedStorageConfigurationException =
                new FailedStorageConfigurationException(
                    message: "Failed configuration storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConfigurationDependencyException =
                new ConfigurationDependencyException(
                    message: "Configuration dependency error occurred, contact support.",
                    innerException: failedStorageConfigurationException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(someConfiguration);

            ConfigurationDependencyException actualConfigurationDependencyException =
                await Assert.ThrowsAsync<ConfigurationDependencyException>(
                    testCode: addConfigurationTask.AsTask);

            // then
            actualConfigurationDependencyException.Should()
                .BeEquivalentTo(expectedConfigurationDependencyException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConfigurationDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfConfigurationAlreadyExistsAndLogItAsync()
        {
            // given
            DateTimeOffset someDate = GetRandomDateTimeOffset();
            Configuration someConfiguration = CreateRandomConfiguration(someDate);

            var duplicateKeyException =
                new DuplicateKeyException(message: "Duplicate key error occurred");

            var alreadyExistsConfigurationException =
                new AlreadyExistsConfigurationException(
                    message: "Configuration already exists error occurred.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedConfigurationDependencyValidationException =
                new ConfigurationDependencyValidationException(
                    message: "Configuration dependency validation error occurred, fix errors and try again.",
                    innerException: alreadyExistsConfigurationException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(someConfiguration);

            ConfigurationDependencyValidationException actualConfigurationDependencyValidationException =
                await Assert.ThrowsAsync<ConfigurationDependencyValidationException>(
                    testCode: addConfigurationTask.AsTask);

            // then
            actualConfigurationDependencyValidationException.Should()
                .BeEquivalentTo(expectedConfigurationDependencyValidationException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyErrorOccurredAndLogItAsync()
        {
            // given
            DateTimeOffset someDate = GetRandomDateTimeOffset();
            Configuration someConfiguration = CreateRandomConfiguration(someDate);
            var dbUpdateException = new DbUpdateException();

            var failedOperationConfigurationException =
                new FailedOperationConfigurationException(
                    message: "Failed operation configuration error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedConfigurationDependencyException =
                new ConfigurationDependencyException(
                    message: "Configuration dependency error occurred, contact support.",
                    innerException: failedOperationConfigurationException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(someConfiguration);

            ConfigurationDependencyException actualConfigurationDependencyException =
                 await Assert.ThrowsAsync<ConfigurationDependencyException>(
                     testCode: addConfigurationTask.AsTask);

            // then
            actualConfigurationDependencyException.Should()
                .BeEquivalentTo(expectedConfigurationDependencyException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConfigurationAsync(
                    It.IsAny<Configuration>()),
                        Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDate = GetRandomDateTimeOffset();
            Configuration someConfiguration = CreateRandomConfiguration(someDate);
            var serviceException = new Exception();

            var failedServiceConfigurationException =
                new FailedServiceConfigurationException(
                    message: "Failed service configuration error occurred, contact support.",
                    innerException: serviceException);

            var expectedConfigurationServiceException = new ConfigurationServiceException(
                message: "Service error occurred, contact support.",
                innerException: failedServiceConfigurationException);

            this.datetimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Configuration> addConfigurationTask =
                this.configurationService.AddConfigurationAsync(someConfiguration);

            var actualConfigurationServiceException =
                await Assert.ThrowsAsync<ConfigurationServiceException>(
                    testCode: addConfigurationTask.AsTask);

            // then
            actualConfigurationServiceException.Should()
                .BeEquivalentTo(expectedConfigurationServiceException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertConfigurationAsync(
                    It.IsAny<Configuration>()),
                        Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
