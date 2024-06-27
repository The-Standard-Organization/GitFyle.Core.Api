// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        void AddSourceConfigurations(EntityTypeBuilder<Source> builder)
        {
            builder.Property(source => source.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(source => source.Name)
                .IsUnique();

            builder.Property(source => source.Url)
                .HasMaxLength(255)
                .IsRequired();
        }
    }
}
