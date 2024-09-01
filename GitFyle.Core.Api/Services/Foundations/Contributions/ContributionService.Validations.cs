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
                (Rule: await IsInvalidAsync(contribution.Title),
                    Parameter: nameof(Contribution.Title)),
                (Rule: await IsInvalidAsync(contribution.ExternalId), 
                    Parameter: nameof(Contribution.ExternalId)),
                (Rule: await IsInvalidAsync(contribution.RepositoryId), 
                    Parameter: nameof(Contribution.RepositoryId)),
                (Rule: await IsInvalidAsync(contribution.ContributorId),
                    Parameter: nameof(Contribution.ContributorId)),
                (Rule: await IsInvalidLengthAsync(contribution.Title, 255), 
                    Parameter: nameof(Contribution.Title)),
                (Rule: await IsInvalidLengthAsync(contribution.ExternalId, 255), 
                    Parameter: nameof(Contribution.ExternalId)),
                (Rule: await IsInvalidAsync(contribution.ContributionTypeId), 
                    Parameter: nameof(Contribution.ContributionTypeId)),
                (Rule: await IsInvalidAsync(contribution.ExternalCreatedAt), 
                    Parameter: nameof(Contribution.ExternalCreatedAt)),
                (Rule: await IsInvalidAsync(contribution.ExternalUpdatedAt), 
                    Parameter: nameof(Contribution.ExternalUpdatedAt)),
                (Rule: await IsInvalidAsync(contribution.ExternalMergedAt), 
                    Parameter: nameof(Contribution.ExternalMergedAt)),

                (Rule: await IsDatesNotSameAsync(
                    createdDate: contribution.ExternalCreatedAt,
                            updatedDate: contribution.ExternalUpdatedAt,
                            nameof(Contribution.ExternalCreatedAt)),

                    Parameter: nameof(Contribution.ExternalUpdatedAt)),

                (Rule: await IsNotRecentAsync(contribution.ExternalCreatedAt), 
                    Parameter: nameof(Contribution.ExternalCreatedAt)));
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
            int pastYears = 10;
            int futureSeconds = 0;
            DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            if (currentDateTime == default)
            {
                return (false, default, default);
            }

            DateTimeOffset startDate = currentDateTime.AddYears(-pastYears);
            DateTimeOffset endDate = currentDateTime.AddSeconds(futureSeconds);
            bool isNotRecent = date < startDate;

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

        private static async ValueTask<dynamic> IsDatesNotSameAsync(
           DateTimeOffset createdDate,
           DateTimeOffset updatedDate,
           string createdDateName) => new
           {
               Condition = createdDate != updatedDate,
               Message = $"Date is not the same as {createdDateName}"
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
