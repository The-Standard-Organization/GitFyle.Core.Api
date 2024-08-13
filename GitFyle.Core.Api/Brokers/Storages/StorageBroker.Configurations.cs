// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Configuration> Configurations { get; set; }

        public async ValueTask<Configuration> InsertConfigurationAsync(Configuration configuration) =>
            await InsertAsync(configuration);

        public async ValueTask<IQueryable<Configuration>> GetAllConfigurationsAsync() =>
            await SelectAllAsync<Configuration>();
    }
}
