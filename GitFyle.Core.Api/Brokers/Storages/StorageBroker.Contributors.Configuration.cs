// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Contributors;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {

        void AddContributorConfigurations(EntityTypeBuilder<Contributor> builder) 
        {
            
            builder.Property(contributor => contributor.ExternalId)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(contributor => contributor.SourceId)
                .IsRequired();

            builder.Property(contributor => contributor.Username)
                .HasMaxLength(255).
                IsRequired();

            builder.Property(contributor => contributor.Name)
                .HasMaxLength(255);

            builder.Property(contributor => contributor.Email)
                .HasMaxLength(255);

            builder.HasIndex(contributor => new
            {
                    contributor.ExternalId,
                    contributor.SourceId,
                    contributor.Username,

            });

            builder.HasOne(x => x.Source)
                .WithMany(x => x.Contributors)
                .HasForeignKey(x => x.SourceId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);

        }

    }
}
