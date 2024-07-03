// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Source> Sources { get; set; }

        public ValueTask<Source> InsertSourceAsync(Source source) =>
            InsertAsync(source);

        public async ValueTask<Source> SelectSourceByIdAsync(Guid sourceId) =>
            await SelectAsync<Source>(sourceId);

        public IQueryable<Source> SelectAllSources() => SelectAll<Source>();


        public async ValueTask<Source> UpdateSourceAsync(Source source) =>
            await UpdateAsync(source);

        public async ValueTask<Source> DeleteSourceAsync(Source source) =>
            await DeleteAsync(source);
    }
}
