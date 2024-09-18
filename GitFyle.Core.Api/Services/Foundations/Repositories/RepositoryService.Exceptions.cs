// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.Repositories
{
    internal partial class RepositoryService
    {
        private delegate ValueTask<Repository> ReturningRepositoryFunction();

        private async ValueTask<Repository> TryCatch(ReturningRepositoryFunction returningRepositoryFunction)
        {
            try
            {
                return await returningRepositoryFunction();
            }
            catch (NullRepositoryException nullRepositoryException)
            {
                throw await CreateAndLogValidationException(nullRepositoryException);
            }
            catch (InvalidRepositoryException invalidRepositoryException)
            {
                throw await CreateAndLogValidationException(invalidRepositoryException);
            }
        }

        private async ValueTask<RepositoryValidationException> CreateAndLogValidationException(
            Xeption exception)
        {
            var RepositoryValidationException = new RepositoryValidationException(
                message: "Repository validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(RepositoryValidationException);

            return RepositoryValidationException;
        }
    }
}