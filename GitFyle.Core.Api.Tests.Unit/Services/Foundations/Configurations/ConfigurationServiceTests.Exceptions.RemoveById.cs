using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Numerics;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someConfigurationId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedStorageConfigurationException =
                new FailedStorageConfigurationException(
                    message: "Failed configuration storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConfigurationDependencyException =
                new ConfigurationDependencyException(
                    message: "Configuration dependency error occurred, contact support.",
                    innerException: failedStorageConfigurationException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectConfigurationByIdAsync(someConfigurationId))
                    .ThrowsAsync(sqlException);
            // when
            ValueTask<Configuration> removeConfigurationByIdTask = 
                this.configurationService.RemoveConfigurationByIdAsync(someConfigurationId);

            ConfigurationDependencyException actualConfigurationDependencyException =
                await Assert.ThrowsAsync<ConfigurationDependencyException>(
                    removeConfigurationByIdTask.AsTask);

            // then
            actualConfigurationDependencyException.Should().BeEquivalentTo(
                expectedConfigurationDependencyException);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectConfigurationByIdAsync(someConfigurationId), 
                    Times.Once());

            this.loggingBrokerMock.Verify(broker => 
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedConfigurationDependencyException))), 
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffsetAsync(), 
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDbConcurrencyOccursAndLogItAsync()
        {
            // given
            Guid someConfigurationId = Guid.NewGuid();

            var dbUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedConfigurationException =
                new LockedConfigurationException(                    
                    message: "Locked configuration record error occurred, please try again.",
                    innerException: dbUpdateConcurrencyException);

            var expectedConfigurationDependencyValidationException =
                new ConfigurationDependencyValidationException(
                    message: "Configuration dependency validation error occurred, fix errors and try again.",
                    innerException: lockedConfigurationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConfigurationByIdAsync(someConfigurationId))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Configuration> removeConfigurationByIdTask =
                this.configurationService.RemoveConfigurationByIdAsync(someConfigurationId);

            ConfigurationDependencyValidationException actualConfigurationDependencyValidationException =
                await Assert.ThrowsAsync<ConfigurationDependencyValidationException>(
                    removeConfigurationByIdTask.AsTask);

            // then
            actualConfigurationDependencyValidationException.Should().BeEquivalentTo(
                expectedConfigurationDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConfigurationByIdAsync(someConfigurationId),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationDependencyValidationException))),
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
