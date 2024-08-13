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
                (Rule: await IsInvalid(source.Id), Parameter: nameof(Source.Id)),
                (Rule: await IsInvalid(source.Name), Parameter: nameof(Source.Name)),
                (Rule: await IsInvalid(source.CreatedBy), Parameter: nameof(Source.CreatedBy)),
                (Rule: await IsInvalid(source.UpdatedBy), Parameter: nameof(Source.UpdatedBy)),
                (Rule: await IsInvalid(source.CreatedDate), Parameter: nameof(Source.CreatedDate)),
                (Rule: await IsInvalid(source.UpdatedDate), Parameter: nameof(Source.UpdatedDate)),
                (Rule: await IsInvalidLength(source.Name, 255), Parameter: nameof(Source.Name)),
                (Rule: await IsInvalidUrl(source.Url), Parameter: nameof(Source.Url)),

                (Rule: await IsValuesNotSame(
                    createBy: source.UpdatedBy,
                    updatedBy: source.CreatedBy,
                    createdByName: nameof(Source.CreatedBy)),

                Parameter: nameof(Source.UpdatedBy)),

                (Rule: await IsDatesNotSame(
                    createdDate: source.CreatedDate,
                    updatedDate: source.UpdatedDate,
                    nameof(Source.CreatedDate)),

                Parameter: nameof(Source.UpdatedDate)),

                (Rule: await IsNotRecent(source.CreatedDate), Parameter: nameof(Source.CreatedDate)));
        }

        private async ValueTask<dynamic> IsNotRecent(DateTimeOffset date) => new
        {
            Condition = await IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private async ValueTask<bool> IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime = await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();
            TimeSpan timeDifference = currentDateTime.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private static async ValueTask<dynamic> IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid"
        };

        private static async ValueTask<dynamic> IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is required"
        };

        private static async ValueTask<dynamic> IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid"
        };

        private static async ValueTask<dynamic> IsInvalidLength(string text, int maxLength) => new
        {
            Condition = await IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static async ValueTask<bool> IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static async ValueTask<dynamic> IsInvalidUrl(string url) => new
        {
            Condition = await IsValidUrl(url) is false,
            Message = "Url is invalid"
        };

        private static async ValueTask<dynamic> IsDatesNotSame(
            DateTimeOffset createdDate,
            DateTimeOffset updatedDate,
            string createdDateName) => new
            {
                Condition = createdDate != updatedDate,
                Message = $"Date is not the same as {createdDateName}"
            };

        private static async ValueTask<dynamic> IsValuesNotSame(
            string createBy,
            string updatedBy,
            string createdByName) => new
            {
                Condition = createBy != updatedBy,
                Message = $"Text is not the same as {createdByName}"
            };

        public static async ValueTask<bool> IsValidUrl(string url)
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

        private static void ValidateSourceIsNotNull(Source source)
        {
            if (source is null)
            {
                throw new NullSourceException(message: "Source is null");
            }
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