// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Sources;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal partial class SourceService : ISourceService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public SourceService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Source> AddSourceAsync(Source source) =>
        TryCatch(async () =>
        {
            ValidateSourceOnAdd(source);
            DoSomeAdditionalInstruction();

            return await this.storageBroker.InsertSourceAsync(source);
        });

        public IQueryable<Source> RetrieveAllSources() =>
        TryCatch(() =>
        {
            DoSomeAdditionalInstruction();

            return this.storageBroker.SelectAllSources();
        });

        private static void DoSomeAdditionalInstruction()
        {
            var x = 1 + 2;
        }
    }
}