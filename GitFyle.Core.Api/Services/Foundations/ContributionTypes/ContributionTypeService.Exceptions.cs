// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace GitFyle.Core.Api.Services.Foundations.ContributionTypes
{
    internal partial class ContributionTypeService
    {
        private delegate ValueTask<ContributionType> ReturningContributionTypeFunction();

        private async ValueTask<ContributionType> TryCatch(ReturningContributionTypeFunction returningContributionTypeFunction)
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
            catch (SqlException sqlException)
            {
                var failedStorageContributionTypeException = new FailedStorageContributionTypeException(
                    message: "Failed storage contributionType error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageContributionTypeException);
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
    }
}
