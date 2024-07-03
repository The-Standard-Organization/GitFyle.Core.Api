// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Repository> Repositories { get; set; }

        public IQueryable<Repository> SelectAllRepositoriesAsync() =>
            SelectAll<Repository>();
    }
}

