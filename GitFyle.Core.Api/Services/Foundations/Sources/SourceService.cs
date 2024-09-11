// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Sources;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal partial class SourceService : ISourceService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public SourceService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Source> AddSourceAsync(Source source) =>
        TryCatch(async () =>
        {
            await ValidateSourceOnAddAsync(source);

            return await this.storageBroker.InsertSourceAsync(source);
        });

        public ValueTask<Source> RetrieveSourceByIdAsync(Guid sourceId) =>
        TryCatch(async () =>
        {
            await ValidateSourceIdAsync(sourceId);

            Source maybeSource =
                await this.storageBroker.SelectSourceByIdAsync(sourceId);

            await ValidateStorageSourceAsync(maybeSource, sourceId);

            return maybeSource;
        });

        public ValueTask<IQueryable<Source>> RetrieveAllSourcesAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllSourcesAsync());

        public ValueTask<Source> ModifySourceAsync(Source source) =>
        TryCatch(async () =>
        {
            await ValidateSourceOnModifyAsync(source);

            Source maybeSource =
                await this.storageBroker.SelectSourceByIdAsync(source.Id);

            await ValidateStorageSourceAsync(maybeSource, source.Id);
            await ValidateAgainstStorageSourceOnModifyAsync(source, maybeSource);

            return await this.storageBroker.UpdateSourceAsync(source);
        });

        public ValueTask<Source> RemoveSourceByIdAsync(Guid sourceId) =>
            throw new NotImplementedException();
    }
}