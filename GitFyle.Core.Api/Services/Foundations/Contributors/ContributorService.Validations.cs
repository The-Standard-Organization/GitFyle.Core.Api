// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Contributors
{
    internal partial class ContributorService
    {
        private async ValueTask ValidateContributorOnAddAsync(Contributor contributor)
        {
            ValidateContributorIsNotNull(contributor);

            Validate(
                (Rule: IsInvalid(contributor.Id),
                    Parameter: nameof(Contributor.Id)),

                (Rule: IsInvalid(contributor.SourceId),
                    Parameter: nameof(Contributor.SourceId)),

                (Rule: IsInvalid(contributor.Username),
                    Parameter: nameof(Contributor.Username)),

                (Rule: IsInvalid(contributor.Email),
                    Parameter: nameof(Contributor.Email)),

                (Rule: IsInvalid(contributor.AvatarUrl),
                    Parameter: nameof(Contributor.AvatarUrl)),

                (Rule: IsInvalid(contributor.Name),
                    Parameter: nameof(Contributor.Name)),

                (Rule: IsInvalid(contributor.CreatedBy),
                    Parameter: nameof(Contributor.CreatedBy)),

                (Rule: IsInvalid(contributor.CreatedDate),
                    Parameter: nameof(Contributor.CreatedDate)),

                (Rule: IsInvalid(contributor.UpdatedBy),
                    Parameter: nameof(Contributor.UpdatedBy)),

                (Rule: IsInvalid(contributor.UpdatedDate),
                    Parameter: nameof(Contributor.UpdatedDate)),

                (Rule: IsInvalidLength(contributor.Username, 255),
                    Parameter: nameof(Contributor.Username)),

                (Rule: IsInvalidLength(contributor.Name, 255),
                    Parameter: nameof(Contributor.Name)),

                (Rule: IsInvalidLength(contributor.Email, 255),
                    Parameter: nameof(Contributor.Email)),

                (Rule: IsInvalidLength(contributor.ExternalId, 255),
                    Parameter: nameof(Contributor.ExternalId)),

                (Rule: IsNotSame(
                    first: contributor.UpdatedBy,
                    second: contributor.CreatedBy,
                    secondName: nameof(Contributor.CreatedBy)),

                Parameter: nameof(Contributor.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: contributor.UpdatedDate,
                    secondDate: contributor.CreatedDate,
                    secondDateName: nameof(Contributor.CreatedDate)),

                Parameter: nameof(Contributor.UpdatedDate)),

                (Rule: await IsNotRecentAsync(contributor.CreatedDate),
                    Parameter: nameof(Contributor.CreatedDate)));
        }

        private async ValueTask ValidateContributorOnModifyAsync(Contributor contributor)
        {
            ValidateContributorIsNotNull(contributor);

            Validate(
                (Rule: IsInvalid(contributor.Id),
                    Parameter: nameof(Contributor.Id)),

                (Rule: IsInvalid(contributor.SourceId),
                     Parameter: nameof(Contributor.SourceId)),

                (Rule: IsInvalid(contributor.Username),
                    Parameter: nameof(Contributor.Username)),

                (Rule: IsInvalid(contributor.Email),
                    Parameter: nameof(Contributor.Email)),

                (Rule: IsInvalid(contributor.AvatarUrl),
                    Parameter: nameof(Contributor.AvatarUrl)),

                (Rule: IsInvalid(contributor.Name),
                     Parameter: nameof(Contributor.Name)),

                (Rule: IsInvalid(contributor.CreatedBy),
                    Parameter: nameof(Contributor.CreatedBy)),

                (Rule: IsInvalid(contributor.CreatedDate),
                    Parameter: nameof(Contributor.CreatedDate)),

                (Rule: IsInvalid(contributor.UpdatedBy),
                    Parameter: nameof(Contributor.UpdatedBy)),

                (Rule: IsInvalid(contributor.UpdatedDate),
                    Parameter: nameof(Contributor.UpdatedDate)),

                (Rule: IsInvalidLength(contributor.Username, 255),
                    Parameter: nameof(Contributor.Username)),

                (Rule: IsInvalidLength(contributor.Name, 255),
                    Parameter: nameof(Contributor.Name)),

                (Rule: IsInvalidLength(contributor.Email, 255),
                    Parameter: nameof(Contributor.Email)),

                (Rule: IsInvalidLength(contributor.ExternalId, 255),
                    Parameter: nameof(Contributor.ExternalId)),

                (Rule: IsSame(
                    firstDate: contributor.UpdatedDate,
                    secondDate: contributor.CreatedDate,
                    secondDateName: nameof(Contributor.CreatedDate)),

                Parameter: nameof(Contributor.UpdatedDate)),

                (Rule: await IsNotRecentAsync(contributor.UpdatedDate),
                    Parameter: nameof(Contributor.UpdatedDate)));
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

        private static void ValidateContributorIsNotNull(Contributor contributor)
        {
            if (contributor is null)
            {
                throw new NullContributorException(message: "Contributor is null");
            }
        }

        private static void ValidateContributorIdAsync(Guid contributorId) =>
           Validate((Rule: IsInvalid(contributorId), Parameter: nameof(Contributor.Id)));

        private static void ValidateStorageContributor(Contributor maybeContributor, Guid contributorId)
        {
            if (maybeContributor is null)
            {
                throw new NotFoundContributorException(
                    message: $"Contributor not found with id: {contributorId}");
            }
        }

        private static void ValidateAgainstStorageContributorOnModify(
             Contributor inputContributor, Contributor storageContributor)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputContributor.CreatedBy,
                    second: storageContributor.CreatedBy,
                    secondName: nameof(Contributor.CreatedBy)),

                Parameter: nameof(Contributor.CreatedBy)),

                (Rule: IsNotSame(
                    firstDate: inputContributor.CreatedDate,
                    secondDate: storageContributor.CreatedDate,
                    secondDateName: nameof(Contributor.CreatedDate)),

                Parameter: nameof(Contributor.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputContributor.UpdatedDate,
                    secondDate: storageContributor.UpdatedDate,
                    secondDateName: nameof(Contributor.UpdatedDate)),

                Parameter: nameof(Contributor.UpdatedDate)));
        }
        private static void ValidateStorageContributorAsync(Contributor maybeContributor, Guid id)
        {
            if (maybeContributor is null)
            {
                throw new NotFoundContributorException(
                    message: $"Contributor not found with id: {id}");
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidContributorException =
                new InvalidContributorException(
                message: "Contributor is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidContributorException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidContributorException.ThrowIfContainsErrors();
        }
    }
}