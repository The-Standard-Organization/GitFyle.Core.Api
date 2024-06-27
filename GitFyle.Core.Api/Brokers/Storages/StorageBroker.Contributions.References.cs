// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Contributions;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        void AddContributionConfigurations(EntityTypeBuilder<Contribution> builder)
        {
            modelBuilder.Entity<Contribution>()
                .HasIndex(contribution => new
                {
                    contribution.RepositoryId,
                    contribution.ExternalId
                })
                .IsUnique();

            modelBuilder.Entity<Contribution>()
                .Property(contribution => contribution.ExternalId)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Contribution>()
                .Property(contribution => contribution.Title)
                .HasMaxLength(255)
                .IsRequired();   
        }
    }
}
