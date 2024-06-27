// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        void AddContributionTypeConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContributionType>()
                .Property(contributionType => contributionType.Name)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<ContributionType>()
                .HasIndex(contributionType => contributionType.Name)
                .IsUnique();

            modelBuilder.Entity<ContributionType>()
                .Property(contributionType => contributionType.Value)
                .IsRequired();
        }
    }
}
