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
                (Rule: await IsInvalidAsync(contribution.Id), 
                    Parameter: nameof(Contribution.Id)),

                (Rule: await IsInvalidAsync(contribution.RepositoryId), 
                    Parameter: nameof(Contribution.RepositoryId)),

                (Rule: await IsInvalidAsync(contribution.ContributorId), 
                    Parameter: nameof(Contribution.ContributorId)),

                (Rule: await IsInvalidAsync(contribution.ContributionTypeId),
                    Parameter: nameof(Contribution.ContributionTypeId)),

                (Rule: await IsInvalidAsync(contribution.ExternalId), 
                    Parameter: nameof(Contribution.ExternalId)),

                (Rule: await IsInvalidAsync(contribution.Title), 
                    Parameter: nameof(Contribution.Title)),

                (Rule: await IsInvalidAsync(contribution.CreatedBy), 
                    Parameter: nameof(Contribution.CreatedBy)),

                (Rule: await IsInvalidAsync(contribution.CreatedDate), 
                    Parameter: nameof(Contribution.CreatedDate)),

                (Rule: await IsInvalidAsync(contribution.UpdatedBy), 
                    Parameter: nameof(Contribution.UpdatedBy)),

                (Rule: await IsInvalidAsync(contribution.UpdatedDate), 
                    Parameter: nameof(Contribution.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(contribution.Title, 255), 
                    Parameter: nameof(Contribution.Title)),

                (Rule: await IsInvalidLengthAsync(contribution.ExternalId, 255),
                    Parameter: nameof(Contribution.ExternalId)),

                (Rule: await IsNotSameAsync(
                    first: contribution.CreatedBy,
                    second: contribution.UpdatedBy,
                    secondName: nameof(Contribution.CreatedBy)),

                Parameter: nameof(Contribution.UpdatedBy)),

                (Rule: await IsNotSameAsync(
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
                (Rule: await IsInvalidAsync(contribution.Id), Parameter: nameof(Contribution.Id)),

                (Rule: await IsInvalidAsync(contribution.RepositoryId), 
                    Parameter: nameof(Contribution.RepositoryId)),

                (Rule: await IsInvalidAsync(contribution.ContributorId), 
                    Parameter: nameof(Contribution.ContributorId)),

                (Rule: await IsInvalidAsync(contribution.ContributionTypeId),
                    Parameter: nameof(Contribution.ContributionTypeId)),

                (Rule: await IsInvalidAsync(contribution.ExternalId), 
                    Parameter: nameof(Contribution.ExternalId)),

                (Rule: await IsInvalidAsync(contribution.Title), 
                    Parameter: nameof(Contribution.Title)),

                (Rule: await IsInvalidAsync(contribution.CreatedBy), 
                    Parameter: nameof(Contribution.CreatedBy)),

                (Rule: await IsInvalidAsync(contribution.CreatedDate), 
                    Parameter: nameof(Contribution.CreatedDate)),

                (Rule: await IsInvalidAsync(contribution.UpdatedBy), 
                    Parameter: nameof(Contribution.UpdatedBy)),

                (Rule: await IsInvalidAsync(contribution.UpdatedDate), 
                    Parameter: nameof(Contribution.UpdatedDate)),

                (Rule: await IsInvalidLengthAsync(contribution.Title, 255), 
                    Parameter: nameof(Contribution.Title)),

                (Rule: await IsInvalidLengthAsync(contribution.ExternalId, 255),
                    Parameter: nameof(Contribution.ExternalId)),

                (Rule: await IsSameAsync(
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

        private static async ValueTask<dynamic> IsInvalidLengthAsync(string text, int maxLength) => new
        {
            Condition = await IsExceedingLengthAsync(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static async ValueTask<bool> IsExceedingLengthAsync(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static async ValueTask<dynamic> IsSameAsync(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static async ValueTask<dynamic> IsNotSameAsync(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not the same as {secondName}"
            };

        private static async ValueTask ValidateContributionIdAsync(Guid contributionId) =>
            Validate((Rule: await IsInvalidAsync(contributionId), Parameter: nameof(Contribution.Id)));

        private static async ValueTask ValidateStorageContributionAsync(Contribution maybeContribution, Guid id)
        {
            if (maybeContribution is null)
            {
                throw new NotFoundContributionException(
                    message: $"Contribution not found with id: {id}");
            }
        }

        private static async ValueTask ValidateAgainstStorageContributionOnModifyAsync(
          Contribution inputContribution, Contribution storageContribution)
        {
            Validate(
                (Rule: await IsNotSameAsync(
                    first: inputContribution.CreatedBy,
                    second: storageContribution.CreatedBy,
                    secondName: nameof(Contribution.CreatedBy)),

                Parameter: nameof(Contribution.CreatedBy)),

                (Rule: await IsNotSameAsync(
                    firstDate: inputContribution.CreatedDate,
                    secondDate: storageContribution.CreatedDate,
                    secondDateName: nameof(Contribution.CreatedDate)),

                Parameter: nameof(Contribution.CreatedDate)),

                (Rule: await IsSameAsync(
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
