// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Repositories
{
    internal partial class RepositoryService
    {
        private void ValidateRepositoryOnAdd(Repository repository)
        {
            ValidateRepositoryIsNotNull(repository);
        }

        private static void ValidateRepositoryIsNotNull(Repository repository)
        {
            if (repository is null)
            {
                throw new NullRepositoryException(message: "Repository is null");
            }
        }
    }
}