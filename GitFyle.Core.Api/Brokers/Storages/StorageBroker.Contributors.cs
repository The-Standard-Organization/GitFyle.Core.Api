using GitFyle.Core.Api.Models.Foundations.Contributor;
using Microsoft.EntityFrameworkCore;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<Contributor> Contributors {  get; set; }
    }
}