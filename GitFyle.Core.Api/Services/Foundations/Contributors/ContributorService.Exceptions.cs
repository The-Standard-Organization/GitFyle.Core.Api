// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Contributors
{
    internal partial class ContributorService
    {
        private delegate ValueTask<Contributor> ReturningContributorFunction();

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
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationContributorException =
                    new FailedOperationContributorException(
                        message: "Failed operation contributor error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationContributorException);
            }
        }

        private async ValueTask<ContributorValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var contributionValidationException = new ContributorValidationException(
                message: "Contributor validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributionValidationException);

            return contributionValidationException;
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

        private async ValueTask<ContributorDependencyException> CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var contributorDependencyException = new ContributorDependencyException(
                message: "Contributor dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributorDependencyException);

            return contributorDependencyException;
        }
    }
}
