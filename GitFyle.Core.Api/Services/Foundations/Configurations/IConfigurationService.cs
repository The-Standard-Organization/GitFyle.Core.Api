// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;

namespace GitFyle.Core.Api.Services.Foundations.Configurations
{
    public interface IConfigurationService
    {
        ValueTask<Configuration> AddConfigurationAsync(Configuration configuration);
    }
}
