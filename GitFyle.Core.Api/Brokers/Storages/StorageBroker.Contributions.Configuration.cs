// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Contributions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        void AddContributionConfigurations(EntityTypeBuilder<Contribution> builder)
        {
            builder
                .Property(contribution => contribution.RepositoryId)
                .IsRequired();

            builder
                .Property(contribution => contribution.ContributorId)
                .IsRequired();

            builder
                .Property(contribution => contribution.ContributionTypeId)
                .IsRequired();

            builder
                .Property(contribution => contribution.ExternalId)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property(contribution => contribution.Title)
                .HasMaxLength(255)
                .IsRequired();

            builder
                .HasIndex(contribution => new
                {
                    contribution.RepositoryId,
                    contribution.ExternalId
                })
                .IsUnique();

            builder
                .HasOne(contribution => contribution.Repository)
                .WithMany(respository => respository.Contributions)
                .HasForeignKey(contribution => contribution.RepositoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(contribution => contribution.Contributor)
                .WithMany(contributor => contributor.Contributions)
                .HasForeignKey(contribution => contribution.ContributorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(contribution => contribution.ContributionType)
                .WithMany(contributionType => contributionType.Contributions)
                .HasForeignKey(contribution => contribution.ContributionTypeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
