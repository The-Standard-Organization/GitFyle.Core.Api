using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
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
        }


        private async ValueTask<ContributionValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var sourceValidationException = new ContributionValidationException(
                message: "Contribution validation error occurred, fix errors and try again.",
                innerException: exception);

            await this.loggingBroker.LogErrorAsync(sourceValidationException);

            return sourceValidationException;
        }

    }
}
