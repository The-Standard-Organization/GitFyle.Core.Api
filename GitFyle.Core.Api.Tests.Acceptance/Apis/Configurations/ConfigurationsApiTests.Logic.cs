﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitFyle.Core.Api.Tests.Acceptance.Models.Configurations;
using RESTFulSense.Exceptions;

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

        [Fact]
        public async Task ShouldPutConfigurationAsync()
        {
            // given
            Configuration randomConfiguration = await PostRandomConfigurationAsync();
            Configuration modifiedConfiguration = UpdateConfigurationRandom(randomConfiguration);

            // when
            await this.gitFyleCoreApiBroker.PutConfigurationAsync(modifiedConfiguration);

            Configuration actualConfiguration =
                await this.gitFyleCoreApiBroker.GetConfigurationByIdAsync(randomConfiguration.Id);

            // then
            actualConfiguration.Should().BeEquivalentTo(modifiedConfiguration);
            await this.gitFyleCoreApiBroker.DeleteConfigurationByIdAsync(actualConfiguration.Id);
        }

        [Fact]
        public async Task ShouldDeleteConfigurationAsync()
        {
            // given
            Configuration randomConfiguration = await PostRandomConfigurationAsync();
            Configuration inputConfiguration = randomConfiguration;
            Configuration expectedConfiguration = inputConfiguration;

            // when 
            Configuration deletedConfiguration =
                await this.gitFyleCoreApiBroker.DeleteConfigurationByIdAsync(inputConfiguration.Id);

            ValueTask<Configuration> getConfigurationByIdTask =
                this.gitFyleCoreApiBroker.GetConfigurationByIdAsync(inputConfiguration.Id);

            // then
            deletedConfiguration.Should().BeEquivalentTo(expectedConfiguration);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(() =>
               getConfigurationByIdTask.AsTask());
        }
    }
}
