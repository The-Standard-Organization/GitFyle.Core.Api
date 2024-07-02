// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Source> Sources { get; set; }

        public ValueTask<Source> InsertSourceAsync(Source source) =>
            InsertAsync(source);

        public async ValueTask<Source> SelectSourceByIdAsync(Guid sourceId)
        {
            using var broker = new StorageBroker(this.configuration);
            broker.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return await Sources.FindAsync(sourceId);
        }
    }
}
