// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.Repositories
{
    internal partial class RepositoryService
    {
        private delegate ValueTask<Repository> ReturningRepositoryFunction();
        private delegate ValueTask<IQueryable<Repository>> ReturningRepositoriesFunction();

        private async ValueTask<Repository> TryCatch(ReturningRepositoryFunction returningRepositoryFunction)
        {
            try
            {
                return await returningRepositoryFunction();
            }
            catch (NullRepositoryException nullRepositoryException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullRepositoryException);
            }
            catch (InvalidRepositoryException invalidRepositoryException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidRepositoryException);
            }
            catch (NotFoundRepositoryException notFoundRepositoryException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundRepositoryException);
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
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var concurrencyGemException =
                    new LockedRepositoryException(
                        message: "Locked repository record error occurred, please try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(concurrencyGemException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationRepositoryException =
                    new FailedOperationRepositoryException(
                        message: "Failed operation repository error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationRepositoryException);
            }
            catch (Exception exception)
            {
                var failedServiceRepositoryException =
                    new FailedServiceRepositoryException(
                        message: "Failed service repository error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceRepositoryException);
            }
        }

        private async ValueTask<IQueryable<Repository>> TryCatch(
            ReturningRepositoriesFunction returningRepositoriesFunction)
        {
            try
            {
                return await returningRepositoriesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageRepositoryException = new FailedStorageRepositoryException(
                    message: "Failed repository storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageRepositoryException);
            }
            catch (Exception exception)
            {
                var failedServiceRepositoryException =
                    new FailedServiceRepositoryException(
                        message: "Failed service repository error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceRepositoryException);
            }
        }

        private async ValueTask<RepositoryValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var RepositoryValidationException = new RepositoryValidationException(
                message: "Repository validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(RepositoryValidationException);

            return RepositoryValidationException;
        }

        private async Task<RepositoryDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var repositoryDependencyException = new RepositoryDependencyException(
                message: "Repository dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(repositoryDependencyException);

            return repositoryDependencyException;
        }

        private async ValueTask<RepositoryDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var RepositoryDependencyValidationException = new RepositoryDependencyValidationException(
                message: "Repository dependency validation error occurred, fix errors and try again.",
                innerException: exception,
                data: exception.Data);

            await this.loggingBroker.LogErrorAsync(RepositoryDependencyValidationException);

            return RepositoryDependencyValidationException;
        }

        private async ValueTask<RepositoryDependencyException> CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var RepositoryDependencyException = new RepositoryDependencyException(
                message: "Repository dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(RepositoryDependencyException);

            return RepositoryDependencyException;
        }

        private async ValueTask<RepositoryServiceException> CreateAndLogServiceExceptionAsync(
           Xeption exception)
        {
            var RepositoryServiceException = new RepositoryServiceException(
                message: "Service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(RepositoryServiceException);

            return RepositoryServiceException;
        }
    }
}