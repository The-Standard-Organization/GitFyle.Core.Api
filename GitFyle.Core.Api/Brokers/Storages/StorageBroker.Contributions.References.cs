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
            builder.HasIndex(contribution => new
                {
                    contribution.RepositoryId,
                    contribution.ExternalId
                })
                .IsUnique();

            builder.Property(contribution => contribution.ExternalId)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(contribution => contribution.Title)
                .HasMaxLength(255)
                .IsRequired();   
        }
    }
}
