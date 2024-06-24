using GitFyle.Core.Api.Models.Foundations.Contributions;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        void SetContributionReferences(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contribution>()
                .HasIndex(contribution => new
                {
                    contribution.RepositoryId,
                    contribution.ExternalId
                }).IsUnique();
        }
    }
}
