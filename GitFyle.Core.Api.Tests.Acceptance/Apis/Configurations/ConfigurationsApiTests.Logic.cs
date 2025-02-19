// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Models.Foundations.Configurations;

namespace GitFyle.Core.Api.Tests.Acceptance.Apis.Configurations
{
    public partial class ConfigurationsApiTests
    {
        [Fact]
        public async Task ShouldPostConfigurationAsync()
        {
            // given
            DateTimeOffset currentDate = DateTimeOffset.UtcNow;
            Configuration randomConfiguration = CreateRandomConfiguration(currentDate);
            Configuration inputConfiguration = randomConfiguration;
            Configuration expectedConfiguration = inputConfiguration;

            // when
            Configuration actualConfiguration =
                await this.gitFyleCoreApiBroker.PostConfigurationAsync(inputConfiguration);

            // then
            actualConfiguration.Should().BeEquivalentTo(expectedConfiguration);
            await this.gitFyleCoreApiBroker.DeleteConfigurationByIdAsync(actualConfiguration.Id);
        }

        [Fact]
        public async Task ShouldGetAllConfigurationsAsync()
        {
            // given
            IEnumerable<Configuration> randomConfigurations = CreateRandomConfigurations();
            IEnumerable<Configuration> inputConfigurations = randomConfigurations;

            foreach (Configuration configuration in inputConfigurations)
            {
                await this.gitFyleCoreApiBroker.PostConfigurationAsync(configuration);
            }

            IEnumerable<Configuration> expectedConfigurations = inputConfigurations;

            // when
            IEnumerable<Configuration> actualConfigurations = await this.gitFyleCoreApiBroker.GetAllConfigurationsAsync();

            // then
            foreach (Configuration expectedConfiguration in expectedConfigurations)
            {
                Configuration actualConfiguration = actualConfigurations.Single(configuration => configuration.Id == expectedConfiguration.Id);
                actualConfiguration.Should().BeEquivalentTo(expectedConfiguration);
                await this.gitFyleCoreApiBroker.DeleteConfigurationByIdAsync(actualConfiguration.Id);
            }
        }
    }
}
