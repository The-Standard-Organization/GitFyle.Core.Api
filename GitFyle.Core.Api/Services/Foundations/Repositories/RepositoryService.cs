// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Repositories;

namespace GitFyle.Core.Api.Services.Foundations.Repositories
{
    internal partial class RepositoryService : IRepositoryService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public RepositoryService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Repository> AddRepositoryAsync(Repository repository) =>
        TryCatch(async () =>
        {
            await ValidateRepositoryOnAddAsync(repository);

            return await this.storageBroker.InsertRepositoryAsync(repository);
        });
    }
}