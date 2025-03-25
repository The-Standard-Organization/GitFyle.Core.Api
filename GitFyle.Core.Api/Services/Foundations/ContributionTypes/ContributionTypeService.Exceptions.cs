// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.ContributionTypes
{
    internal partial class ContributionTypeService
    {
        private delegate ValueTask<ContributionType> ReturningContributionTypeFunction();
        private delegate ValueTask<IQueryable<ContributionType>> ReturningContributionTypesFunction();

        private async ValueTask<ContributionType> TryCatch(
            ReturningContributionTypeFunction returningContributionTypeFunction)
        {
            try
            {
                return await returningContributionTypeFunction();
            }
            catch (NullContributionTypeException nullContributionTypeException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullContributionTypeException);
            }
            catch (InvalidContributionTypeException invalidContributionTypeException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidContributionTypeException);
            }
            catch (NotFoundContributionTypeException notFoundContributionTypeException)
            {
                throw await CreateAndLogValidationExceptionAsync(notFoundContributionTypeException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageContributionTypeException = new FailedStorageContributionTypeException(
                    message: "Failed storage contributionType error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageContributionTypeException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsContributionTypeException =
                    new AlreadyExistsContributionTypeException(
                        message: "ContributionType already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsContributionTypeException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidReferenceContributionTypeException =
                    new InvalidReferenceContributionTypeException(
                        message: "Invalid contributionType reference error occurred.",
                        innerException: foreignKeyConstraintConflictException,
                        data: foreignKeyConstraintConflictException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(invalidReferenceContributionTypeException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var concurrencyGemException =
                    new LockedContributionTypeException(
                        message: "Locked contributionType record error occurred, please try again.",
                        innerException: dbUpdateConcurrencyException,
                        data: dbUpdateConcurrencyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(concurrencyGemException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedOperationContributionTypeException =
                    new FailedOperationContributionTypeException(
                        message: "Failed operation contributionType error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(failedOperationContributionTypeException);
            }
            catch (Exception exception)
            {
                var failedServiceContributionTypeException =
                    new FailedServiceContributionTypeException(
                        message: "Failed service contributionType error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceContributionTypeException);
            }
        }

        private async ValueTask<IQueryable<ContributionType>> TryCatch(
            ReturningContributionTypesFunction returningContributionTypesFunction)
        {
            try
            {
                return await returningContributionTypesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStorageContributionTypeException = new FailedStorageContributionTypeException(
                    message: "Failed contributionType storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageContributionTypeException);
            }
            catch (Exception exception)
            {
                var failedServiceContributionTypeException =
                    new FailedServiceContributionTypeException(
                        message: "Failed service contributionType error occurred, contact support.",
                        innerException: exception);

                throw await CreateAndLogServiceExceptionAsync(failedServiceContributionTypeException);
            }
        }

        private async ValueTask<ContributionTypeValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var contributionValidationException = new ContributionTypeValidationException(
                message: "ContributionType validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributionValidationException);

            return contributionValidationException;
        }

        private async ValueTask<ContributionTypeDependencyException>
            CreateAndLogCriticalDependencyExceptionAsync(Xeption exception)
        {
            var contributionTypeDependencyException = new ContributionTypeDependencyException(
                message: "ContributionType dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(contributionTypeDependencyException);

            return contributionTypeDependencyException;
        }

        private async ValueTask<ContributionTypeDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var contributionTypeDependencyValidationException = new ContributionTypeDependencyValidationException(
                message: "ContributionType dependency validation error occurred, fix errors and try again.",
                innerException: exception,
                data: exception.Data);

            await this.loggingBroker.LogErrorAsync(contributionTypeDependencyValidationException);

            return contributionTypeDependencyValidationException;
        }

        private async ValueTask<ContributionTypeDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var contributionTypeDependencyException = new ContributionTypeDependencyException(
                message: "ContributionType dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributionTypeDependencyException);

            return contributionTypeDependencyException;
        }

        private async ValueTask<ContributionTypeServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var contributionTypeServiceException = new ContributionTypeServiceException(
                message: "Service error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(contributionTypeServiceException);

            return contributionTypeServiceException;
        }
    }
}
