using GitFyle.Core.Api.Models.Foundations.Contributions;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        void AddContributionConfigurations(ModelBuilder modelBuilder)
        {
            int maxStringLength = 255;

            modelBuilder.Entity<Contribution>()
                .HasIndex(contribution => new
                {
                    contribution.RepositoryId,
                    contribution.ExternalId
                }).IsUnique();

            modelBuilder.Entity<Contribution>()
                .Property(contribution => contribution.ExternalId)
                .HasMaxLength(maxStringLength)
                .IsRequired();

            modelBuilder.Entity<Contribution>()
                .Property(contribution => contribution.Title)
                .HasMaxLength(maxStringLength)
                .IsRequired();   
        }
    }
}
