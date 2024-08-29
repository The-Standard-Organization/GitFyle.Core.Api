using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using Microsoft.Data.SqlClient;
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
            catch (SqlException sqlException)
            {
                var failedStorageContributionException = new FailedStorageContributionException(
                    message: "Failed contribution storage error occurred, contact support.",
                    innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(failedStorageContributionException);
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

        private async ValueTask<ContributionDependencyException> CreateAndLogCriticalDependencyExceptionAsync(
        Xeption exception)
        {
            var contributionDependencyException = new ContributionDependencyException(
                message: "Contribution dependency error occurred, contact support.",
                innerException: exception);

            await this.loggingBroker.LogCriticalAsync(contributionDependencyException);

            return contributionDependencyException;
        }

    }
}
