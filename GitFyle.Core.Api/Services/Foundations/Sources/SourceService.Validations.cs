// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Sources
{
    internal partial class SourceService : ISourceService
    {
        private void ValidateSourceOnAdd(Source source)
        {
            ValidateSourceIsNotNull(source);

            Validate(
                (Rule: IsInvalid(source.Id), Parameter: nameof(Source.Id)),
                (Rule: IsInvalid(source.Name), Parameter: nameof(Source.Name)),
                (Rule: IsInvalid(source.CreatedBy), Parameter: nameof(Source.CreatedBy)),
                (Rule: IsInvalid(source.UpdatedBy), Parameter: nameof(Source.UpdatedBy)),
                (Rule: IsInvalid(source.CreatedDate), Parameter: nameof(Source.CreatedDate)),
                (Rule: IsInvalid(source.UpdatedDate), Parameter: nameof(Source.UpdatedDate)),
                (Rule: IsInvalidUrl(source.Url), Parameter: nameof(Source.Url)));
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

        private static dynamic IsInvalidUrl(string url) => new
        {
            Condition = IsValidUrl(url) is false,
            Message = "Url is invalid"
        };

        public static bool IsValidUrl(string url)
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