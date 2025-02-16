using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            DateTimeOffset randomDate = DateTimeOffset.UtcNow;
            Configuration randomConfiguration = CreateRandomConfiguration(randomDate);
            Configuration inputConfiguration = randomConfiguration;
            Configuration expectedConfiguration = inputConfiguration;

            // when
            Configuration actualConfiguration =
                await this.gitFyleCoreApiBroker.PostConfigurationAsync(inputConfiguration);

            // then
            actualConfiguration.Should().BeEquivalentTo(expectedConfiguration);
            await this.gitFyleCoreApiBroker.DeleteConfigurationByIdAsync(actualConfiguration.Id);
        }
    }
}
