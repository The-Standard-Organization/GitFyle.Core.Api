using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;

namespace GitFyle.Core.Api.Tests.Acceptance.Brokers
{
    public partial class GitFyleCoreApiBroker
    {
        private const string ConfigurationRelativeUrl = "api/configurations";

        public async ValueTask<Configuration> PostConfigurationAsync(Configuration configuration) =>
            await this.apiFactoryClient.PostContentAsync(ConfigurationRelativeUrl, configuration);

        public async ValueTask<Configuration> DeleteConfigurationByIdAsync(Guid configurationId) =>
            await this.apiFactoryClient.DeleteContentAsync<Configuration>($"{ConfigurationRelativeUrl}/{configurationId}");
    }
}
