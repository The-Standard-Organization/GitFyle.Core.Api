// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
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
    }
}