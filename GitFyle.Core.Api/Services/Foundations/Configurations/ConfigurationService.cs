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
            await ValidateConfigurationOnAdd(configuration);

            return await this.storageBroker.InsertConfigurationAsync(configuration);
        });

        public ValueTask<Configuration> RetrieveConfigurationByIdAsync(Guid configurationId) =>
        TryCatch(async () =>
        {
            await ValidateConfigurationIdAsync(configurationId);

            Configuration maybeConfiguration =
                await this.storageBroker.SelectConfigurationByIdAsync(configurationId);

            await ValidateStorageConfigurationAsync(maybeConfiguration, configurationId);

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

            await ValidateStorageConfigurationAsync(maybeConfiguration, configuration.Id);
            await ValidateAgainstStorageConfigurationOnModifyAsync(configuration, maybeConfiguration);

            return await this.storageBroker.UpdateConfigurationAsync(configuration);
        });

        public async ValueTask<Configuration> RemoveConfigurationByIdAsync(Guid configurationId)
        {
            Configuration maybeConfiguration = 
                await this.storageBroker.SelectConfigurationByIdAsync(configurationId);

            return await this.storageBroker.DeleteConfigurationAsync(maybeConfiguration);
        }
    }
}
