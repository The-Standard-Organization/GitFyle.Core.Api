// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;

namespace GitFyle.Core.Api.Services.Foundations.Repositories
{
    public interface IRepositoryService
    {
        ValueTask<Repository> AddRepositoryAsync(Repository repository);
    }
}