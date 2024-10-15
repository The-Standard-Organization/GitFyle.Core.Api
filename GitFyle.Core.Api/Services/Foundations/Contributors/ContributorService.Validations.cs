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
                    Parameter: nameof(Contributor.ExternalId)));
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

        private static void ValidateContributorIsNotNull(Contributor contributor)
        {
            if (contributor is null)
            {
                throw new NullContributorException(message: "Contributor is null");
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