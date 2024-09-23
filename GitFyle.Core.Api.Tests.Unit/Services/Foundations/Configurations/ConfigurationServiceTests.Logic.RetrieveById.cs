// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldRetrieveConfigurationByIdAsync()
        {
            // given
            Configuration randomConfiguration = CreateRandomConfiguration();
            Configuration storageConfiguration = randomConfiguration;
            Configuration expectedConfiguration = storageConfiguration.DeepClone();

            this.storageBrokerMock.Setup(broker =>
            broker.SelectConfigurationByIdAsync(randomConfiguration.Id))
                .ReturnsAsync(storageConfiguration);

            // when
            Configuration actualConfiguration =
                await this.configurationService.RetrieveConfigurationByIdAsync(
                    randomConfiguration.Id);

            // then
            actualConfiguration.Should().BeEquivalentTo(expectedConfiguration);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectConfigurationByIdAsync(randomConfiguration.Id),
                    Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
