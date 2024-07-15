// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal partial class SourceService : ISourceService
    {
        private delegate ValueTask<Source> ReturningSourceFunction();
        private delegate IQueryable<Source> ReturningSourcesFunction();

        private async ValueTask<Source> TryCatch(ReturningSourceFunction returningSourceFunction)
        {
            try
            {
                return await returningSourceFunction();
            }
            catch (NullSourceException nullSourceException)
            {
                throw CreateAndLogValidationException(nullSourceException);
            }
        }

        private IQueryable<Source> TryCatch(ReturningSourcesFunction returningSourcesFunction)
        {
            try
            {
                return returningSourcesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedSourceStorageException =
                    new FailedSourceStorageException(
                        message: "Failed source storage error occurred, contact support.",
                        innerException: sqlException);

                throw CreateAndLogCriticalDependencyException(failedSourceStorageException);
            }
            catch (Exception exception)
            {
                var failedSourceServiceException =
                    new FailedSourceServiceException(
                        message: "Failed source service occurred, please contact support",
                        innerException: exception);

                throw CreateAndLogServiceException(failedSourceServiceException);
            }
        }

        private SourceValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var sourceValidationException = new SourceValidationException(
                message: "Source validation error occurred, fix errors and try again.",
                innerException: exception);

            this.loggingBroker.LogError(sourceValidationException);

            return sourceValidationException;
        }

        private SourceDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var sourceDependencyException =
                new SourceDependencyException(
                    message: "Source dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogCritical(sourceDependencyException);

            return sourceDependencyException;
        }

        private SourceServiceException CreateAndLogServiceException(Xeption exception)
        {
            var sourceServiceException =
                new SourceServiceException(
                    message: "Source service error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError(sourceServiceException);

            return sourceServiceException;
        }
    }
}