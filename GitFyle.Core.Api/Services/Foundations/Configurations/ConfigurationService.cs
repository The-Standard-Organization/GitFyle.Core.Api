using System.Threading.Tasks;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Configurations;

namespace GitFyle.Core.Api.Services.Foundations.Configurations
{
    internal class ConfigurationService : IConfigurationService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public ConfigurationService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Configuration> AddConfigurationAsync(Configuration configuration) =>
            await this.storageBroker.InsertConfigurationAsync(configuration);
    }
}
