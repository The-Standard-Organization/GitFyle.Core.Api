using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using Moq;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllConfigurationsAsync()
        {
            // given
            IQueryable<Configuration> randomConfigurations = CreateRandomConfigurations();
            IQueryable<Configuration> storageConfigurations = randomConfigurations;
            IQueryable<Configuration> expectedConfigurations = storageConfigurations;

            this.storageBrokerMock.Setup(broker => 
                broker.SelectAllConfigurationsAsync())
                    .ReturnsAsync(storageConfigurations);

            // when
            IQueryable<Configuration> actualConfigurations = 
                await this.configurationService.RetrieveAllConfigurationsAsync();

            // then
            actualConfigurations.Should().BeEquivalentTo(expectedConfigurations);

            this.storageBrokerMock.Verify(broker => 
                broker.SelectAllConfigurationsAsync(), 
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.datetimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
