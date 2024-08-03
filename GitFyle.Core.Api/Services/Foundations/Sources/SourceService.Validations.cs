// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using Validations;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal partial class SourceService : ISourceService
    {
        private async ValueTask ValidateSourceOnAdd(Source source)
        {
            ValidateSourceIsNotNull(source);

            await this.validationBroker.Validate<InvalidSourceException>(
                message: "Source is invalid, fix the errors and try again.",
                (Rule: ValidationRules.IsInvalid(source.Id), Parameter: nameof(Source.Id)),
                (Rule: ValidationRules.IsInvalid(source.Name), Parameter: nameof(Source.Name)),
                (Rule: IsInvalidUrl(source.Url), Parameter: nameof(Source.Url)),
                (Rule: ValidationRules.IsInvalidMaxLength(source.Name, 255), Parameter: nameof(Source.Name)),
                (Rule: ValidationRules.IsInvalid(source.CreatedDate), Parameter: nameof(Source.CreatedDate)),
                (Rule: ValidationRules.IsInvalid(source.CreatedBy), Parameter: nameof(Source.CreatedBy)),
                (Rule: ValidationRules.IsInvalid(source.UpdatedDate), Parameter: nameof(Source.UpdatedDate)),
                (Rule: ValidationRules.IsInvalid(source.UpdatedBy), Parameter: nameof(Source.UpdatedBy)),

                (Rule: IsNotSameAs(
                    first: source.UpdatedDate,
                    second: source.CreatedDate,
                    secondName: nameof(Source.CreatedDate)),
                Parameter: nameof(Source.UpdatedDate)),

                (Rule: ValidationRules.IsNotSameAs(
                    first: source.UpdatedBy,
                    second: source.CreatedBy,
                    secondName: nameof(Source.CreatedBy)),
                Parameter: nameof(Source.UpdatedBy)));
        }

        internal static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid",
            Values = new object[] { id }
        };

        internal static dynamic IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Url is invalid",
            Values = new object[] { name }
        };

        internal static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid",
            Values = new object[] { date }
        };

        internal static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters",
            Values = new object[] { text, maxLength }
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        internal static dynamic IsInvalidUrl(string url) => new
        {
            Condition = IsValidUrl(url) is false,
            Message = "Url is invalid",
            Values = new object[] { url }
        };

        internal static dynamic IsNotSameAs(
            DateTimeOffset first,
            DateTimeOffset second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Date is not the same as {secondName}",
                Values = new object[]
                {
                    first,
                    second,
                    secondName
                }
            };

        internal static dynamic IsNotSameAs(
           string first,
           string second,
           string secondName) => new
           {
               Condition = first != second,
               Message = $"Text is not the same as {secondName}",
               Values = new object[] { first, second, secondName }
           };

        internal static bool IsValidUrl(string url)
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