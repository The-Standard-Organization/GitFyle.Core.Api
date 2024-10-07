// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal partial class SourceService
    {
        private async ValueTask ValidateSourceOnAddAsync(Source source)
        {
            ValidateSourceIsNotNull(source);

            Validate(
                (Rule: IsInvalid(source.Id), Parameter: nameof(Source.Id)),
                (Rule: IsInvalid(source.Name), Parameter: nameof(Source.Name)),
                (Rule: IsInvalid(source.CreatedBy), Parameter: nameof(Source.CreatedBy)),
                (Rule: IsInvalid(source.CreatedDate), Parameter: nameof(Source.CreatedDate)),
                (Rule: IsInvalid(source.UpdatedBy), Parameter: nameof(Source.UpdatedBy)),
                (Rule: IsInvalid(source.UpdatedDate), Parameter: nameof(Source.UpdatedDate)),
                (Rule: IsInvalidLength(source.Name, 255), Parameter: nameof(Source.Name)),
                (Rule: IsInvalidUrl(source.Url), Parameter: nameof(Source.Url)),

                (Rule: IsNotSame(
                    first: source.UpdatedBy,
                    second: source.CreatedBy,
                    secondName: nameof(Source.CreatedBy)),

                Parameter: nameof(Source.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: source.UpdatedDate,
                    secondDate: source.CreatedDate,
                    secondDateName: nameof(Source.CreatedDate)),

                Parameter: nameof(Source.UpdatedDate)),

                (Rule: await IsNotRecentAsync(source.CreatedDate), Parameter: nameof(Source.CreatedDate)));
        }

        private async ValueTask ValidateSourceOnModifyAsync(Source source)
        {
            ValidateSourceIsNotNull(source);

            Validate(
                (Rule: IsInvalid(source.Id), Parameter: nameof(Source.Id)),
                (Rule: IsInvalid(source.Name), Parameter: nameof(Source.Name)),
                (Rule: IsInvalid(source.CreatedBy), Parameter: nameof(Source.CreatedBy)),
                (Rule: IsInvalid(source.CreatedDate), Parameter: nameof(Source.CreatedDate)),
                (Rule: IsInvalid(source.UpdatedBy), Parameter: nameof(Source.UpdatedBy)),
                (Rule: IsInvalid(source.UpdatedDate), Parameter: nameof(Source.UpdatedDate)),
                (Rule: IsInvalidLength(source.Name, 255), Parameter: nameof(Source.Name)),
                (Rule: IsInvalidUrl(source.Url), Parameter: nameof(Source.Url)),

                (Rule: IsSame(
                    firstDate: source.UpdatedDate,
                    secondDate: source.CreatedDate,
                    secondDateName: nameof(Source.CreatedDate)),

                Parameter: nameof(Source.UpdatedDate)),

                (Rule: await IsNotRecentAsync(source.UpdatedDate), Parameter: nameof(Source.UpdatedDate)));
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

            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

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

        private static dynamic IsInvalidUrl(string url) => new
        {
            Condition = IsValidUrl(url) is false,
            Message = "Url is invalid"
        };

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

        private static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
            {
                return false;
            }

            return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
        }

        private static void ValidateSourceId(Guid sourceId) =>
            Validate((Rule: IsInvalid(sourceId), Parameter: nameof(Source.Id)));

        private static void ValidateSourceIsNotNull(Source source)
        {
            if (source is null)
            {
                throw new NullSourceException(message: "Source is null");
            }
        }

        private static void ValidateStorageSource(Source maybeSource, Guid id)
        {
            if (maybeSource is null)
            {
                throw new NotFoundSourceException(
                    message: $"Source not found with id: {id}");
            }
        }

        private static void ValidateAgainstStorageSourceOnModify(
            Source inputSource, Source storageSource)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputSource.CreatedBy,
                    second: storageSource.CreatedBy,
                    secondName: nameof(Source.CreatedBy)),

                Parameter: nameof(Source.CreatedBy)),

                (Rule: IsNotSame(
                    firstDate: inputSource.CreatedDate,
                    secondDate: storageSource.CreatedDate,
                    secondDateName: nameof(Source.CreatedDate)),

                Parameter: nameof(Source.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputSource.UpdatedDate,
                    secondDate: storageSource.UpdatedDate,
                    secondDateName: nameof(Source.UpdatedDate)),

                Parameter: nameof(Source.UpdatedDate)));
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidSourceException = new InvalidSourceException(
                message: "Source is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidSourceException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidSourceException.ThrowIfContainsErrors();
        }
    }
}