// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;

namespace GitFyle.Core.Api.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<Configuration> InsertConfigurationAsync(Configuration configuration);
        ValueTask<IQueryable<Configuration>> GetAllConfigurationsAsync();
        ValueTask<Configuration> SelectConfigurationByIdAsync(Guid configurationId);
        ValueTask<Configuration> UpdateConfigurationAsync(Configuration configuration);
        ValueTask<Configuration> DeleteConfigurationAsync(Configuration configuration);
    }
}
