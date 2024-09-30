// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Configuration someConfiguration = CreateRandomConfiguration();
            Exception sqlException = CreateSqlException();

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
            ValueTask<Configuration> modifyConfigurationTask =
                this.configurationService.ModifyConfigurationAsync(someConfiguration);

            ConfigurationDependencyException actualConfigurationDependencyException =
                await Assert.ThrowsAsync<ConfigurationDependencyException>(
                    modifyConfigurationTask.AsTask);

            // then
            actualConfigurationDependencyException.Should()
                .BeEquivalentTo(expectedConfigurationDependencyException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConfigurationByIdAsync(someConfiguration.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConfigurationDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = CreateRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Configuration randomConfiguration = CreateRandomConfiguration(randomDateTimeOffset);

            randomConfiguration.CreatedDate =
                randomDateTimeOffset.AddMinutes(minutesInPast);

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
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConfigurationByIdAsync(randomConfiguration.Id))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<Configuration> modifyConfigurationTask =
                this.configurationService.ModifyConfigurationAsync(randomConfiguration);

            ConfigurationDependencyException actualConfigurationDependencyException =
                await Assert.ThrowsAsync<ConfigurationDependencyException>(
                    modifyConfigurationTask.AsTask);

            // then
            actualConfigurationDependencyException.Should()
                .BeEquivalentTo(expectedConfigurationDependencyException);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConfigurationByIdAsync(randomConfiguration.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateConfigurationAsync(It.IsAny<Configuration>()),
                    Times.Never);

            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
