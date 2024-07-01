// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EFxceptions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker : EFxceptionsContext, IStorageBroker
    {
        private readonly IConfiguration configuration;

        public StorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.Database.Migrate();
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                this.configuration.GetConnectionString(
                    name: "DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }

        private async ValueTask<T> InsertAsync<T>(T entity)
        {
            var broker = new StorageBroker(this.configuration);
            broker.Entry(entity).State = EntityState.Added;
            await broker.SaveChangesAsync();

            return entity;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddContributionTypeConfigurations(modelBuilder.Entity<ContributionType>());            
            AddContributorConfigurations(modelBuilder.Entity<Contributor>());
            AddRepositoryConfigurations(modelBuilder.Entity<Repository>());
            AddSourceConfigurations(modelBuilder.Entity<Source>());
        }
    }
}
