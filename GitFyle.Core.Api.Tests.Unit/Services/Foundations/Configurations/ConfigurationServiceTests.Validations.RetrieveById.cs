// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid id = Guid.Empty;

            var invalidConfigurationException = 
                new InvalidConfigurationException(
                    message: "Configuration is invalid, fix the errors and try again.");

            invalidConfigurationException.AddData(
                key: nameof(Configuration.Id), 
                values: "Id is invalid.");

            var expectedConfigurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.", 
                    innerException: invalidConfigurationException);

            // when
            ValueTask<Configuration> retrieveConfigurationByIdTask = 
                this.configurationService.RetrieveConfigurationByIdAsync(id);

            ConfigurationValidationException actualConfigurationValidationException = 
                await Assert.ThrowsAsync<ConfigurationValidationException>(
                    retrieveConfigurationByIdTask.AsTask);
            // then
            actualConfigurationValidationException.Should().BeEquivalentTo(
                expectedConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker => 
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedConfigurationValidationException))), 
                        Times.Once);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectConfigurationByIdAsync(It.IsAny<Guid>()),
                        Times.Never);

            this.datetimeBrokerMock.Verify(broker => 
                broker.GetCurrentDateTimeOffsetAsync(), 
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
