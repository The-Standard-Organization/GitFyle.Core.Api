﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitFyle.Core.Api.Tests.Acceptance.Models.Configurations;

namespace GitFyle.Core.Api.Tests.Acceptance.Brokers
{
    public partial class GitFyleCoreApiBroker
    {
        private const string ConfigurationRelativeUrl = "api/configurations";

        public async ValueTask<Configuration> PostConfigurationAsync(Configuration configuration) =>
            await this.apiFactoryClient.PostContentAsync(ConfigurationRelativeUrl, configuration);

        public async ValueTask<Configuration> GetConfigurationByIdAsync(Guid configurationId) =>
            await this.apiFactoryClient.GetContentAsync<Configuration>($"{ConfigurationRelativeUrl}/{configurationId}");

        public async ValueTask<IEnumerable<Configuration>> GetAllConfigurationsAsync() =>
            await this.apiFactoryClient.GetContentAsync<IEnumerable<Configuration>>(ConfigurationRelativeUrl);

        public async ValueTask<Configuration> PutConfigurationAsync(Configuration configuration) =>
            await this.apiFactoryClient.PutContentAsync(ConfigurationRelativeUrl, configuration);

        public async ValueTask<Configuration> DeleteConfigurationByIdAsync(Guid configurationId) =>
            await this.apiFactoryClient.DeleteContentAsync<Configuration>($"{ConfigurationRelativeUrl}/{configurationId}");
    }
}
