// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;

namespace GitFyle.Core.Api.Services.Foundations.Repositories
{
    public interface IRepositoryService
    {
        ValueTask<Repository> AddRepositoryAsync(Repository repository);
        ValueTask<IQueryable<Repository>> RetrieveAllRepositoriesAsync();
        ValueTask<Repository> RetrieveRepositoryByIdAsync(Guid repositoryId);
        ValueTask<Repository> ModifyRepositoryAsync(Repository repository);
        ValueTask<Repository> RemoveRepositoryByIdAsync(Guid repositoryId);
    }
}