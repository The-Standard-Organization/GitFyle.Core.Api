// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using Xeptions;

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
    }
}
