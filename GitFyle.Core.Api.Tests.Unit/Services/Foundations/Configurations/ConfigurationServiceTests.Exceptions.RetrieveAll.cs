// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSQLExceptionOccursAndLogItAsync()
        {
            // given
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
                broker.SelectAllConfigurationsAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<Configuration>> retrieveAllConfigurationsTask =
                this.configurationService.RetrieveAllConfigurationsAsync();

            // then
            ConfigurationDependencyException actualConfigurationDependencyException =
                await Assert.ThrowsAsync<ConfigurationDependencyException>(
                    testCode: retrieveAllConfigurationsTask.AsTask);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllConfigurationsAsync(),
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
        public async Task ShouldThrowServiceErrorOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Exception serviceError = new Exception();

            var failedServiceConfigurationException =
                new FailedServiceConfigurationException(
                    message: "Failed service configuration error occurred, contact support.",
                    innerException: serviceError);

            var expectedConfigurationServiceException = 
                new ConfigurationServiceException(
                    message: "Service error occurred, contact support.",
                    innerException: failedServiceConfigurationException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectAllConfigurationsAsync())
                    .ThrowsAsync(serviceError);

            // when
            ValueTask<IQueryable<Configuration>> retrieveAllConfigurationTask =
                this.configurationService.RetrieveAllConfigurationsAsync();

            ConfigurationServiceException actualConfigurationServiceException =
                await Assert.ThrowsAsync<ConfigurationServiceException>(
                    testCode: retrieveAllConfigurationTask.AsTask);

            // then
            actualConfigurationServiceException.Should()
                .BeEquivalentTo(expectedConfigurationServiceException);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectAllConfigurationsAsync(), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationServiceException))), 
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
