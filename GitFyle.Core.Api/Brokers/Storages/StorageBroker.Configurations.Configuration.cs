// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        private void AddConfigurationConfigurations(EntityTypeBuilder<Configuration> builder)
        {
            builder
                .Property(configuration => configuration.Name)
                .HasMaxLength(450)
                .IsRequired();

            builder
                .HasIndex(configuration => configuration.Name)
                .IsUnique();

            builder
                .Property(configuration => configuration.Value)
                .IsRequired();
        }
    }
}
