// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Configurations
{
    internal partial class ConfigurationService
    {
        private void ValidateConfigurationOnAdd(Configuration configuration)
        {
            ValidateConfigurationIsNotNull(configuration);
        }

        private void ValidateConfigurationIsNotNull(Configuration configuration) 
        {
            if (configuration == null)
            {
                throw new NullConfigurationException(message: "Configuration is null");
            }
        }
    }
}
