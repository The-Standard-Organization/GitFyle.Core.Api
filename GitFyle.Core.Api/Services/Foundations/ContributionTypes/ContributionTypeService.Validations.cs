// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.ContributionTypes
{
    internal partial class ContributionTypeService
    {
        private async ValueTask ValidateContributionTypeOnAddAsync(ContributionType contributionType)
        {
            ValidateContributionTypeIsNotNull(contributionType);

            Validate(
                (Rule: IsInvalid(contributionType.Id),
                    Parameter: nameof(ContributionType.Id)),

                (Rule: IsInvalid(contributionType.Name),
                    Parameter: nameof(ContributionType.Name)),

                (Rule: IsInvalid(contributionType.CreatedBy),
                    Parameter: nameof(ContributionType.CreatedBy)),

                (Rule: IsInvalid(contributionType.CreatedDate),
                    Parameter: nameof(ContributionType.CreatedDate)),

                (Rule: IsInvalid(contributionType.UpdatedBy),
                    Parameter: nameof(ContributionType.UpdatedBy)),

                (Rule: IsInvalid(contributionType.UpdatedDate),
                    Parameter: nameof(ContributionType.UpdatedDate)),

                (Rule: IsInvalidLength(contributionType.Name, 255),
                    Parameter: nameof(ContributionType.Name)),

                (Rule: IsNotSame(
                    first: contributionType.UpdatedBy,
                    second: contributionType.CreatedBy,
                    secondName: nameof(ContributionType.CreatedBy)),

                Parameter: nameof(ContributionType.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: contributionType.UpdatedDate,
                    secondDate: contributionType.CreatedDate,
                    secondDateName: nameof(ContributionType.CreatedDate)),

                Parameter: nameof(ContributionType.UpdatedDate)),

                (Rule: await IsNotRecentAsync(contributionType.CreatedDate),
                    Parameter: nameof(ContributionType.CreatedDate)));
        }

        private async ValueTask ValidateContributionTypeOnModifyAsync(ContributionType contributionType)
        {
            ValidateContributionTypeIsNotNull(contributionType);

            Validate(
                (Rule: IsInvalid(contributionType.Id),
                    Parameter: nameof(ContributionType.Id)),

                (Rule: IsInvalid(contributionType.Name),
                    Parameter: nameof(ContributionType.Name)),

                (Rule: IsInvalid(contributionType.CreatedBy),
                    Parameter: nameof(ContributionType.CreatedBy)),

                (Rule: IsInvalid(contributionType.CreatedDate),
                    Parameter: nameof(ContributionType.CreatedDate)),

                (Rule: IsInvalid(contributionType.UpdatedBy),
                    Parameter: nameof(ContributionType.UpdatedBy)),

                (Rule: IsInvalid(contributionType.UpdatedDate),
                    Parameter: nameof(ContributionType.UpdatedDate)),

                (Rule: IsInvalidLength(contributionType.Name, 255),
                    Parameter: nameof(ContributionType.Name)),

                (Rule: IsSame(
                    firstDate: contributionType.UpdatedDate,
                    secondDate: contributionType.CreatedDate,
                    secondDateName: nameof(ContributionType.CreatedDate)),

                Parameter: nameof(ContributionType.UpdatedDate)),

                (Rule: await IsNotRecentAsync(contributionType.UpdatedDate),
                    Parameter: nameof(ContributionType.UpdatedDate)));
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
            Condition = IsExceedingLengthAsync(text, maxLength),
            Message = $"Text exceeds max length of {maxLength} characters"
        };

        private static bool IsExceedingLengthAsync(string text, int maxLength) =>
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

        private static void ValidateContributionTypeIdAsync(Guid contributionTypeId) =>
            Validate((Rule: IsInvalid(contributionTypeId), Parameter: nameof(ContributionType.Id)));

        private static void ValidateStorageContributionTypeAsync(ContributionType maybeContributionType, Guid id)
        {
            if (maybeContributionType is null)
            {
                throw new NotFoundContributionTypeException(
                    message: $"ContributionType not found with id: {id}");
            }
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

        private static void ValidateContributionTypeId(Guid contributionTypeId) =>
            Validate((Rule: IsInvalid(contributionTypeId), Parameter: nameof(ContributionType.Id)));

        private static void ValidateContributionTypeIsNotNull(ContributionType contributionType)
        {
            if (contributionType is null)
            {
                throw new NullContributionTypeException(message: "ContributionType is null");
            }
        }

        private static void ValidateStorageContributionType(ContributionType maybeContributionType, Guid contributionTypeId)
        {
            if (maybeContributionType is null)
            {
                throw new NotFoundContributionTypeException(
                    message: $"ContributionType not found with id: {contributionTypeId}");
            }
        }

        private static void ValidateAgainstStorageContributionTypeOnModify(
             ContributionType inputContributionType, ContributionType storageContributionType)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputContributionType.CreatedBy,
                    second: storageContributionType.CreatedBy,
                    secondName: nameof(ContributionType.CreatedBy)),

                Parameter: nameof(ContributionType.CreatedBy)),

                (Rule: IsNotSame(
                    firstDate: inputContributionType.CreatedDate,
                    secondDate: storageContributionType.CreatedDate,
                    secondDateName: nameof(ContributionType.CreatedDate)),

                Parameter: nameof(ContributionType.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputContributionType.UpdatedDate,
                    secondDate: storageContributionType.UpdatedDate,
                    secondDateName: nameof(ContributionType.UpdatedDate)),

                Parameter: nameof(ContributionType.UpdatedDate)));
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidContributionTypeException =
                new InvalidContributionTypeException(
                message: "ContributionType is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidContributionTypeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidContributionTypeException.ThrowIfContainsErrors();
        }
    }
}