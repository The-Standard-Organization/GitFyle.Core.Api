// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.Configurations
{
    internal partial class ConfigurationService
    {
        private delegate ValueTask<Configuration> ReturningConfigurationFunction();

        private async ValueTask<Configuration> TryCatch(ReturningConfigurationFunction returningConfigurationFunction)
        {
            try
            {
                return await returningConfigurationFunction();
            }
            catch (NullConfigurationException nullConfigurationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullConfigurationException);
            }
            catch (InvalidConfigurationException invalidationConfigurationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidationConfigurationException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageConfigurationException =
                    new FailedStorageConfigurationException(
                        message: "Failed configuration storage exception, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageConfigurationException);
            }
        }

        private async ValueTask<ConfigurationDependencyException> CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var configurationDependencyException = 
                new ConfigurationDependencyException(
                    message: "Configuration dependency error occurred, contact support.", 
                    innerException: exception);  

            await this.loggingBroker.LogCriticalAsync(configurationDependencyException);

            return configurationDependencyException;
        }

        private async ValueTask<ConfigurationValidationException> CreateAndLogValidationExceptionAsync(Xeption innerException)
        {
            var configurationValidationException =
                new ConfigurationValidationException(
                    message: "Configuration validation error occurred, fix the errors and try again.",
                    innerException: innerException);

            await this.loggingBroker.LogErrorAsync(configurationValidationException);

            return configurationValidationException;
        }
    }
}
