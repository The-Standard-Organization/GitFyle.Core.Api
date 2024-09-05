using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Configuration someConfiguration = 
                CreateRandomConfiguration(DateTimeOffset.UtcNow);

            SqlException sqlException = CreateSqlException();

            var failedStorageConfigurationException =
                new FailedStorageConfigurationException(
                    message: "Failed configuration storage exception, contact support.",
                    innerException: sqlException);

            var expectedConfigurationDependencyException = new ConfigurationDependencyException(
                message: "Configuration dependency error occurred, contact support.",
                innerException: failedStorageConfigurationException);

            this.datetimeBrokerMock.Setup(broker => 
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<Configuration> addConfigurationTask = 
                this.configurationService.AddConfigurationAsync(someConfiguration);

            ConfigurationDependencyException actualConfigurationDependencyException = 
                await Assert.ThrowsAsync<ConfigurationDependencyException>(addConfigurationTask.AsTask);

            //then
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

            var alreadyExistsConfigurationException = new AlreadyExistsConfigurationException(
                message: "Configuration already exists error occurred.",
                innerException: duplicateKeyException,
                data: duplicateKeyException.Data
                );

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
                await Assert.ThrowsAsync<ConfigurationDependencyValidationException>(addConfigurationTask.AsTask);

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
    }
}
