// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Contributors;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Contributor> Contributors {  get; set; }

        public async ValueTask<Contributor> UpdateContributorAsync(Contributor contributor) =>
            await UpdateAsync(contributor);
    }
}