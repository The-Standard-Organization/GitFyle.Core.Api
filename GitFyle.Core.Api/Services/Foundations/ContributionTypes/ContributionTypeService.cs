// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;

namespace GitFyle.Core.Api.Services.Foundations.ContributionTypes
{
    internal partial class ContributionTypeService : IContributionTypeService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ContributionTypeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ContributionType> AddContributionTypeAsync(ContributionType contributionType) =>
        TryCatch(async () =>
        {
            await ValidateContributionTypeOnAddAsync(contributionType);
            return await this.storageBroker.InsertContributionTypeAsync(contributionType);
        });

        public ValueTask<IQueryable<ContributionType>> RetrieveAllContributionTypesAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllContributionTypesAsync());

        public ValueTask<ContributionType> RetrieveContributionTypeByIdAsync(Guid contributionTypeId) =>
        TryCatch(async () =>
        {
            ValidateContributionTypeIdAsync(contributionTypeId);

            ContributionType maybeContributionType =
                await this.storageBroker.SelectContributionTypeByIdAsync(contributionTypeId);

            await ValidateStorageContributionTypeAsync(maybeContributionType, contributionTypeId);

            return maybeContributionType;
        });
    }
}