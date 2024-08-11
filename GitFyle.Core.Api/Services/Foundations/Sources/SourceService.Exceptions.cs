// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal partial class SourceService : ISourceService
    {
        private delegate ValueTask<Source> ReturningSourceFunction();

        private async ValueTask<Source> TryCatch(ReturningSourceFunction returningSourceFunction)
        {
            try
            {
                return await returningSourceFunction();
            }
            catch (NullSourceException nullSourceException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullSourceException);
            }
            catch (InvalidSourceException invalidSourceException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidSourceException);
            }
            catch (SqlException sqlException)
            {
                var failedStorageSourceException = new FailedStorageSourceException(
                    message: "Failed source storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageSourceException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsSourceException =
                    new AlreadyExistsSourceException(
                        message: "Source already exists error occurred.",
                        innerException: duplicateKeyException,
                        data: duplicateKeyException.Data);

                throw await CreateAndLogDependencyValidationExceptionAsync(alreadyExistsSourceException);
            }
        }

        private async ValueTask<SourceValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var sourceValidationException = new SourceValidationException(
                message: "Source validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(sourceValidationException);

            return sourceValidationException;
        }

        private async ValueTask<SourceDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var sourceDependencyException = new SourceDependencyException(
                message: "Source dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(sourceDependencyException);

            return sourceDependencyException;
        }

        private async ValueTask<SourceDependencyValidationException> CreateAndLogDependencyValidationExceptionAsync(
            Xeption exception)
        {
            var sourceDependencyValidationException = new SourceDependencyValidationException(
                message: "Source dependency validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(sourceDependencyValidationException);

            return sourceDependencyValidationException;
        }
    }
}