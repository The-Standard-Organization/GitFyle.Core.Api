using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Contributions
{
    internal partial class ContributionService
    {
        private async ValueTask ValidateContributionOnAddAsync(Contribution contribution)
        {
            ValidateContributionIsNotNull(contribution);
        }

        private static void ValidateContributionIsNotNull(Contribution contribution)
        {
            if (contribution is null)
            {
                throw new NullContributionException(message: "Contribution is null");
            }
        }
    }
}
