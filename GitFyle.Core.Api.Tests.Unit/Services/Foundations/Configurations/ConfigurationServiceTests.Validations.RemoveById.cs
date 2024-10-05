using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfConfigurationIdIsInvalidAndLogItAsync()
        {
            // given
            Guid someConfigurationId = Guid.Empty;

            var invalidConfigurationException =
                new InvalidConfigurationException(
                    message: "Configuration is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.Id),
                values: "Id is invalid.");

            ConfigurationValidationException expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: invalidConfigurationException);

            // when
            ValueTask<Configuration> removeConfigurationByIdTask =
                this.configurationService.RemoveConfigurationByIdAsync(someConfigurationId);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    removeConfigurationByIdTask.AsTask);

            // then
            actualConfigurationValidationException.Should().BeEquivalentTo(
                expectedConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationValidationException))), 
                        Times.Once);

            this.datetimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffsetAsync(), 
                    Times.Never);

            this.storageBrokerMock.Verify(broker => 
                broker.InsertConfigurationAsync(It.IsAny<Configuration>()), 
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfNotFoundAndLogItAsync()
        {
            // given
            Guid someConfigurationId = Guid.NewGuid();
            Configuration noConfiguration = null;

            var notFoundConfigurationException = 
                new NotFoundConfigurationException(
                    message: $"Configuration not found with id: {someConfigurationId}");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: notFoundConfigurationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConfigurationByIdAsync(someConfigurationId))
                    .ReturnsAsync(noConfiguration);

            // when
            ValueTask<Configuration> removeConfigurationByIdTask = 
                this.configurationService.RemoveConfigurationByIdAsync(someConfigurationId);

            ConfigurationValidationException actualConfigurationValidationException =
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    removeConfigurationByIdTask.AsTask);

            // then
            actualConfigurationValidationException.Should().BeEquivalentTo(
                expectedConfigurationValidationException);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectConfigurationByIdAsync(someConfigurationId), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationValidationException))), 
                        Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.DeleteConfigurationAsync(
                    It.IsAny<Configuration>()), 
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
