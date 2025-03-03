// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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

            Validate(
                (Rule: IsInvalid(contribution.Id),
                    Parameter: nameof(Contribution.Id)),

                (Rule: IsInvalid(contribution.RepositoryId),
                    Parameter: nameof(Contribution.RepositoryId)),

                (Rule: IsInvalid(contribution.ContributorId),
                    Parameter: nameof(Contribution.ContributorId)),

                (Rule: IsInvalid(contribution.ContributionTypeId),
                    Parameter: nameof(Contribution.ContributionTypeId)),

                (Rule: IsInvalid(contribution.ExternalId),
                    Parameter: nameof(Contribution.ExternalId)),

                (Rule: IsInvalid(contribution.Title),
                    Parameter: nameof(Contribution.Title)),

                (Rule: IsInvalid(contribution.CreatedBy),
                    Parameter: nameof(Contribution.CreatedBy)),

                (Rule: IsInvalid(contribution.CreatedDate),
                    Parameter: nameof(Contribution.CreatedDate)),

                (Rule: IsInvalid(contribution.UpdatedBy),
                    Parameter: nameof(Contribution.UpdatedBy)),

                (Rule: IsInvalid(contribution.UpdatedDate),
                    Parameter: nameof(Contribution.UpdatedDate)),

                (Rule: IsInvalidLength(contribution.Title, 255),
                    Parameter: nameof(Contribution.Title)),

                (Rule: IsInvalidLength(contribution.ExternalId, 255),
                    Parameter: nameof(Contribution.ExternalId)),

                (Rule: IsNotSame(
                    first: contribution.CreatedBy,
                    second: contribution.UpdatedBy,
                    secondName: nameof(Contribution.CreatedBy)),

                Parameter: nameof(Contribution.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: contribution.CreatedDate,
                    secondDate: contribution.UpdatedDate,
                    nameof(Contribution.CreatedDate)),

                Parameter: nameof(Contribution.UpdatedDate)),

                (Rule: await IsNotRecentAsync(contribution.CreatedDate),
                    Parameter: nameof(Contribution.CreatedDate)));
        }

        private async ValueTask ValidateContributionOnModifyAsync(Contribution contribution)
        {
            ValidateContributionIsNotNull(contribution);

            Validate(
                (Rule: IsInvalid(contribution.Id), Parameter: nameof(Contribution.Id)),

                (Rule: IsInvalid(contribution.RepositoryId),
                    Parameter: nameof(Contribution.RepositoryId)),

                (Rule: IsInvalid(contribution.ContributorId),
                    Parameter: nameof(Contribution.ContributorId)),

                (Rule: IsInvalid(contribution.ContributionTypeId),
                    Parameter: nameof(Contribution.ContributionTypeId)),

                (Rule: IsInvalid(contribution.ExternalId),
                    Parameter: nameof(Contribution.ExternalId)),

                (Rule: IsInvalid(contribution.Title),
                    Parameter: nameof(Contribution.Title)),

                (Rule: IsInvalid(contribution.CreatedBy),
                    Parameter: nameof(Contribution.CreatedBy)),

                (Rule: IsInvalid(contribution.CreatedDate),
                    Parameter: nameof(Contribution.CreatedDate)),

                (Rule: IsInvalid(contribution.UpdatedBy),
                    Parameter: nameof(Contribution.UpdatedBy)),

                (Rule: IsInvalid(contribution.UpdatedDate),
                    Parameter: nameof(Contribution.UpdatedDate)),

                (Rule: IsInvalidLength(contribution.Title, 255),
                    Parameter: nameof(Contribution.Title)),

                (Rule: IsInvalidLength(contribution.ExternalId, 255),
                    Parameter: nameof(Contribution.ExternalId)),

                (Rule: IsSame(
                    firstDate: contribution.UpdatedDate,
                    secondDate: contribution.CreatedDate,
                    secondDateName: nameof(Contribution.CreatedDate)),

                Parameter: nameof(Contribution.UpdatedDate)),

                (Rule: await IsNotRecentAsync(contribution.UpdatedDate),
                    Parameter: nameof(Contribution.UpdatedDate)));
        }

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date)
        {
            var (isNotRecent, startDate, endDate) = await IsDateNotRecentAsync(date);

            return new
            {
                Condition = isNotRecent,
                Message = $"Date is not recent. Expected a value between {startDate} and {endDate} but found {date}"
            };
        }

        private async ValueTask<(bool IsNotRecent, DateTimeOffset StartDate, DateTimeOffset EndDate)>
            IsDateNotRecentAsync(DateTimeOffset date)
        {
            int pastSeconds = 60;
            int futureSeconds = 0;
            DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            if (currentDateTime == default)
            {
                return (false, default, default);
            }

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            DateTimeOffset startDate = currentDateTime.AddSeconds(-pastSeconds);
            DateTimeOffset endDate = currentDateTime.AddSeconds(futureSeconds);
            bool isNotRecent = timeDifference.TotalSeconds is > 60 or < 0;

            return (isNotRecent, startDate, endDate);
        }

        private static void ValidateContributionIsNotNull(Contribution contribution)
        {
            if (contribution is null)
            {
                throw new NullContributionException(message: "Contribution is null");
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static dynamic IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid"
        };

        private static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not the same as {secondName}"
            };

        private static void ValidateContributionId(Guid contributionId) =>
            Validate((Rule: IsInvalid(contributionId), Parameter: nameof(Contribution.Id)));

        private static void ValidateStorageContribution(Contribution maybeContribution, Guid id)
        {
            if (maybeContribution is null)
            {
                throw new NotFoundContributionException(
                    message: $"Contribution not found with id: {id}");
            }
        }

        private static void ValidateAgainstStorageContributionOnModify(
          Contribution inputContribution, Contribution storageContribution)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputContribution.CreatedBy,
                    second: storageContribution.CreatedBy,
                    secondName: nameof(Contribution.CreatedBy)),

                Parameter: nameof(Contribution.CreatedBy)),

                (Rule: IsNotSame(
                    firstDate: inputContribution.CreatedDate,
                    secondDate: storageContribution.CreatedDate,
                    secondDateName: nameof(Contribution.CreatedDate)),

                Parameter: nameof(Contribution.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputContribution.UpdatedDate,
                    secondDate: storageContribution.UpdatedDate,
                    secondDateName: nameof(Contribution.UpdatedDate)),

                Parameter: nameof(Contribution.UpdatedDate)));
        }

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
