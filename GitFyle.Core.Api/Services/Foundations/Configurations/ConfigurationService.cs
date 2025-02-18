// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Configurations;

namespace GitFyle.Core.Api.Services.Foundations.Configurations
{
    internal partial class ConfigurationService : IConfigurationService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public ConfigurationService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Configuration> AddConfigurationAsync(Configuration configuration) =>
        TryCatch(async () =>
        {
            await ValidateConfigurationOnAddAsync(configuration);

            return await this.storageBroker.InsertConfigurationAsync(configuration);
        });

        public ValueTask<Configuration> RetrieveConfigurationByIdAsync(Guid configurationId) =>
        TryCatch(async () =>
        {
            ValidateConfigurationId(configurationId);

            Configuration maybeConfiguration =
                await this.storageBroker.SelectConfigurationByIdAsync(configurationId);

            ValidateStorageConfiguration(maybeConfiguration, configurationId);

            return maybeConfiguration;
        });

        public ValueTask<IQueryable<Configuration>> RetrieveAllConfigurationsAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllConfigurationsAsync());

        public ValueTask<Configuration> ModifyConfigurationAsync(Configuration configuration) =>
        TryCatch(async () =>
        {
            await ValidateConfigurationOnModify(configuration);

            Configuration maybeConfiguration =
                await this.storageBroker.SelectConfigurationByIdAsync(configuration.Id);

            ValidateStorageConfiguration(maybeConfiguration, configuration.Id);
            ValidateAgainstStorageConfigurationOnModify(configuration, maybeConfiguration);

            return await this.storageBroker.UpdateConfigurationAsync(configuration);
        });

        public ValueTask<Configuration> RemoveConfigurationByIdAsync(Guid configurationId) =>
        TryCatch(async () =>
        {
            ValidateConfigurationId(configurationId);

            Configuration maybeConfiguration =
                await this.storageBroker.SelectConfigurationByIdAsync(configurationId);

            ValidateStorageConfiguration(maybeConfiguration, configurationId);

            return await this.storageBroker.DeleteConfigurationAsync(maybeConfiguration);
        });
    }
}
