// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        void AddContributionTypeConfigurations(EntityTypeBuilder<ContributionType> builder)
        {
            builder.Property(contributionType => contributionType.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(contributionType => contributionType.Name)
                .IsUnique();

            builder.Property(contributionType => contributionType.Value)
                .IsRequired();
        }
    }
}
