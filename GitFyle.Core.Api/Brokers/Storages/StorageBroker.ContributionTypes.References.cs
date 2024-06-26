using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        void SetContributionTypeReferences(ModelBuilder modelBuilder)
        {
            int maxStringLength = 255;

            modelBuilder.Entity<ContributionType>()
                .Property(contributionType => contributionType.Name)
                .HasMaxLength(maxStringLength)
                .IsRequired();

            modelBuilder.Entity<ContributionType>()
                .Property(contributionType => contributionType.Value)
                .IsRequired();
        }
    }
}
