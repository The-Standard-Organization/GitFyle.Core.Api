// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.Contributions
{
    internal partial class ContributionService
    {
        private delegate ValueTask<Contribution> ReturningContributionFunction();

        private async ValueTask<Contribution> TryCatch(ReturningContributionFunction returningContributionFunction)
        {
            try
            {
                return await returningContributionFunction();
            }
            catch (NullContributionException nullContributionException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullContributionException);
            }
            catch (InvalidContributionException invalidContributionException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidContributionException);
            }
            catch (NotFoundContributionException notFoundContributionException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundContributionException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageContributionException = new FailedStorageContributionException(
                    message: "Failed contribution storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageContributionException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsContributionException =
                    new AlreadyExistsContributionException(
                        message: "Contribution already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsContributionException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var concurrencyGemException =
                    new LockedContributionException(
                        message: "Locked contribution record error occurred, please try again.",
                        innerException: dbUpdateConcurrencyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(concurrencyGemException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationContributionException =
                    new FailedOperationContributionException(
                        message: "Failed operation contribution  error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationContributionException);
            }
            catch (Exception exception)
            {
                var failedServiceContributionException =
                    new FailedServiceContributionException(
                        message: "Failed service contribution error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceContributionException);
            }
        }

        private async ValueTask<ContributionValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var contributionValidationException = new ContributionValidationException(
                message: "Contribution validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributionValidationException);

            return contributionValidationException;
        }

        private async ValueTask<ContributionDependencyException>
            CreateAndLogCriticalDependencyExceptionAsync(
                Xeption exception)
        {
            var contributionDependencyException = new ContributionDependencyException(
                message: "Contribution dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(contributionDependencyException);

            return contributionDependencyException;
        }

        private async ValueTask<ContributionDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var contributionDependencyValidationException = new ContributionDependencyValidationException(
                message: "Contribution dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributionDependencyValidationException);

            return contributionDependencyValidationException;
        }

        private async ValueTask<ContributionDependencyException>
            CreateAndLogDependencyExceptionAsync(
                Xeption exception)
        {
            var contributionDependencyException = new ContributionDependencyException(
                message: "Contribution dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributionDependencyException);

            return contributionDependencyException;
        }

        private async ValueTask<ContributionServiceException>
            CreateAndLogServiceExceptionAsync(
                Xeption exception)
        {
            var contributionServiceException = new ContributionServiceException(
                message: "Service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributionServiceException);

            return contributionServiceException;
        }
    }
}
