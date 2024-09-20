// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Microsoft.Data.SqlClient;
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
            catch (SqlException sqlException)
            {
                var failedStorageRepositoryException = new FailedStorageRepositoryException(
                    message: "Failed storage repository error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageRepositoryException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsRepositoryException =
                    new AlreadyExistsRepositoryException(
                        message: "Repository already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsRepositoryException);
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

        private async Task<Exception> CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var repositoryDependencyException = new RepositoryDependencyException(
                message: "Repository dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(repositoryDependencyException);

            return repositoryDependencyException;
        }

        private async ValueTask<RepositoryDependencyValidationException> CreateAndLogDependencyValidationExceptionAsync(
            Xeption exception)
        {
            var RepositoryDependencyValidationException = new RepositoryDependencyValidationException(
                message: "Repository dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(RepositoryDependencyValidationException);

            return RepositoryDependencyValidationException;
        }
    }
}