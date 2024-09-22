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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            var someConfigurationId = Guid.NewGuid();
            var sqlException = CreateSqlException();

            var failedStorageConfigurationException = 
                new FailedStorageConfigurationException(
                    message: "Failed configuration storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedConfigurationDependencyException = 
                new ConfigurationDependencyException(
                    message:"Configuration dependency error occurred, contact support.", 
                    innerException: failedStorageConfigurationException);

            this.storageBrokerMock.Setup(broker => 
                broker.SelectConfigurationByIdAsync(someConfigurationId))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Configuration> retrieveConfigurationByIdTask = 
                this.configurationService.RetrieveConfigurationByIdAsync(someConfigurationId);

            ConfigurationDependencyException actualConfigurationDependencyException = 
                await Assert.ThrowsAsync<ConfigurationDependencyException>(
                    retrieveConfigurationByIdTask.AsTask);

            // then
            actualConfigurationDependencyException.Should().BeEquivalentTo(
                expectedConfigurationDependencyException);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectConfigurationByIdAsync(someConfigurationId), 
                    Times.Once);

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
    }
}
