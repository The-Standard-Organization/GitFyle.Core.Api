// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldRemoveConfigurationAsync()
        {
            // given
            Guid someConfigurationId = Guid.NewGuid();
            Configuration randomConfiguration = CreateRandomConfiguration();
            Configuration storageConfiguration = randomConfiguration;
            Configuration inputConfiguration = storageConfiguration;
            Configuration removedConfiguration = inputConfiguration;
            Configuration expectedConfiguration = removedConfiguration.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectConfigurationByIdAsync(someConfigurationId))
                    .ReturnsAsync(storageConfiguration);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteConfigurationAsync(inputConfiguration))
                    .ReturnsAsync(removedConfiguration);

            // when
            Configuration actualConfiguration =
                await this.configurationService.RemoveConfigurationByIdAsync(someConfigurationId);

            // then
            actualConfiguration.Should().BeEquivalentTo(expectedConfiguration);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConfigurationByIdAsync(someConfigurationId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteConfigurationAsync(storageConfiguration),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
