// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.Contributors
{
    internal partial class ContributorService
    {
        private delegate ValueTask<Contributor> ReturningContributorFunction();
        private delegate ValueTask<IQueryable<Contributor>> ReturningContributorsFunction();

        private async ValueTask<Contributor> TryCatch(ReturningContributorFunction returningContributorFunction)
        {
            try
            {
                return await returningContributorFunction();
            }
            catch (NullContributorException nullContributorException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullContributorException);
            }
            catch (InvalidContributorException invalidContributorException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidContributorException);
            }
            catch (NotFoundContributorException notFoundContributorException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundContributorException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageContributorException = new FailedStorageContributorException(
                    message: "Failed storage contributor error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageContributorException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsContributorException =
                    new AlreadyExistsContributorException(
                        message: "Contributor already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsContributorException);

            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {

                var concurrencyGemException =
                    new LockedContributorException(
                        message: "Locked contributor record error occurred, please try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(concurrencyGemException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationContributorException =
                    new FailedOperationContributorException(
                        message: "Failed operation contributor error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationContributorException);
            }
            catch (Exception exception)
            {
                var failedServiceContributorException =
                    new FailedServiceContributorException(
                        message: "Failed service contributor error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceContributorException);
            }
        }

        private async ValueTask<IQueryable<Contributor>> TryCatch(
            ReturningContributorsFunction returningContributorsFunction)
        {
            try
            {
                return await returningContributorsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageContributorException = new FailedStorageContributorException(
                    message: "Failed contributor storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageContributorException);
            }
            catch (Exception exception)
            {
                var failedServiceContributorException =
                    new FailedServiceContributorException(
                        message: "Failed service contributor error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceContributorException);
            }
        }

        private async ValueTask<ContributorValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var contributorValidationException = new ContributorValidationException(
                message: "Contributor validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributorValidationException);

            return contributorValidationException;
        }

        private async ValueTask<ContributorDependencyException>
            CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var contributorDependencyException = new ContributorDependencyException(
                message: "Contributor dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(contributorDependencyException);

            return contributorDependencyException;
        }

        private async ValueTask<ContributorDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var contributorDependencyValidationException = new ContributorDependencyValidationException(
                message: "Contributor dependency validation error occurred, fix errors and try again.",
                innerException: exception,
                data: exception.Data);

            await this.loggingBroker.LogErrorAsync(contributorDependencyValidationException);

            return contributorDependencyValidationException;
        }

        private async ValueTask<ContributorDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var contributorDependencyException = new ContributorDependencyException(
                message: "Contributor dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributorDependencyException);

            return contributorDependencyException;
        }

        private async ValueTask<ContributorServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var contributorServiceException = new ContributorServiceException(
                message: "Service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributorServiceException);

            return contributorServiceException;
        }
    }
}
