// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Contributor> Contributors {  get; set; }

        public ValueTask<Contributor> InsertContributorAsync(Contributor contributor) =>
            InsertAsync(contributor); 

        public async ValueTask<IQueryable<Contributor>> SelectAllContributorsAsync() => 
            await Task.FromResult(SelectAll<Contributor>()); 
      
        public async ValueTask<Contributor> SelectContributorByIdAsync(Guid contributorId) =>
            await SelectAsync<Contributor>(contributorId);

        public async ValueTask<Contributor> UpdateContributorAsync(Contributor contributor) =>
            await UpdateAsync(contributor);

        public async ValueTask<Contributor> DeleteContributorAsync(Contributor contributor) => 
            await DeleteAsync(contributor);
    }
}
