using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Contributions;

namespace GitFyle.Core.Api.Services.Foundations.Contributions
{
    internal partial class ContributionService
    {
        private async ValueTask ValidateContributionOnAddAsync(Contribution contribution)
        {
            ValidateContributionIsNotNull(contribution);

            Validate(
                (Rule: await IsInvalidAsync(contribution.Id), Parameter: nameof(Contribution.Id)),
                (Rule: await IsInvalidAsync(contribution.Title), Parameter: nameof(Contribution.Title)),
                (Rule: await IsInvalidAsync(contribution.ExternalId), Parameter: nameof(Contribution.ExternalId)),
                (Rule: await IsInvalidAsync(contribution.RepositoryId), Parameter: nameof(Contribution.RepositoryId)),
                (Rule: await IsInvalidAsync(contribution.ContributorId), Parameter: nameof(Contribution.ContributorId)),
                (Rule: await IsInvalidAsync(contribution.ContributionTypeId), Parameter: nameof(Contribution.ContributionTypeId)),
                (Rule: await IsInvalidAsync(contribution.ExternalCreatedAt), Parameter: nameof(Contribution.ExternalCreatedAt)),
                (Rule: await IsInvalidAsync(contribution.ExternalUpdatedAt), Parameter: nameof(Contribution.ExternalUpdatedAt)),
                (Rule: await IsInvalidAsync(contribution.ExternalMergedAt), Parameter: nameof(Contribution.ExternalMergedAt)));
        }

        private static void ValidateContributionIsNotNull(Contribution contribution)
        {
            if (contribution is null)
            {
                throw new NullContributionException(message: "Contribution is null");
            }
        }

        private static async ValueTask<dynamic> IsInvalidAsync(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static async ValueTask<dynamic> IsInvalidAsync(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is required"
        };

        private static async ValueTask<dynamic> IsInvalidAsync(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidContributionException = new InvalidContributionException(
                message: "Contribution is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidContributionException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidContributionException.ThrowIfContainsErrors();
        }
    }
}
