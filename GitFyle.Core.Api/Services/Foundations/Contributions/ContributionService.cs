// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Contributions;

namespace GitFyle.Core.Api.Services.Foundations.Contributions
{
    internal partial class ContributionService : IContributionService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ContributionService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Contribution> AddContributionAsync(Contribution contribution) =>
        TryCatch(async () =>
        {
            await ValidateContributionOnAddAsync(contribution);

            return await this.storageBroker.InsertContributionAsync(contribution);
        });  
        
        public ValueTask<IQueryable<Contribution>> RetrieveAllContributionsAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllContributionsAsync());

        public ValueTask<Contribution> RetrieveContributionByIdAsync(Guid contributionId) =>
        TryCatch(async () =>
        {
            await ValidateContributionIdAsync(contributionId);

            Contribution maybeContribution =
                await this.storageBroker.SelectContributionByIdAsync(contributionId);

            await ValidateStorageContributionAsync(maybeContribution, contributionId);

            return maybeContribution;
        });

        public ValueTask<Contribution> ModifyContributionAsync(Contribution contribution) =>
        TryCatch(async () =>
        {
            await ValidateContributionOnModifyAsync(contribution);
            Contribution maybeContribution =
                await this.storageBroker.SelectContributionByIdAsync(contribution.Id);

            await ValidateStorageContributionAsync(maybeContribution, contribution.Id);
            await ValidateAgainstStorageContributionOnModifyAsync(contribution, maybeContribution);

            return await this.storageBroker.UpdateContributionAsync(contribution);
        });
    }
}