// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Repositories
{
    internal partial class RepositoryService
    {
        private async ValueTask ValidateRepositoryOnAddAsync(Repository repository)
        {
            ValidateRepositoryIsNotNull(repository);

            Validate(
                (Rule: await IsInvalidAsync(repository.Id), Parameter: nameof(Repository.Id)),
                (Rule: await IsInvalidAsync(repository.Name), Parameter: nameof(Repository.Name)),
                (Rule: await IsInvalidAsync(repository.Owner), Parameter: nameof(Repository.Owner)),
                (Rule: await IsInvalidAsync(repository.ExternalId), Parameter: nameof(Repository.ExternalId)),
                (Rule: await IsInvalidAsync(repository.SourceId), Parameter: nameof(Repository.SourceId)),
                (Rule: await IsInvalidAsync(repository.Token), Parameter: nameof(Repository.Token)),
                (Rule: await IsInvalidAsync(repository.TokenExpireAt), Parameter: nameof(Repository.TokenExpireAt)),
                (Rule: await IsInvalidAsync(repository.Description), Parameter: nameof(Repository.Description)),

                (Rule: await IsInvalidAsync(repository.ExternalCreatedAt),
                    Parameter: nameof(Repository.ExternalCreatedAt)),

                (Rule: await IsInvalidAsync(repository.ExternalUpdatedAt),
                    Parameter: nameof(Repository.ExternalUpdatedAt)),

                (Rule: await IsInvalidAsync(repository.CreatedBy), Parameter: nameof(Repository.CreatedBy)),
                (Rule: await IsInvalidAsync(repository.CreatedDate), Parameter: nameof(Repository.CreatedDate)),
                (Rule: await IsInvalidAsync(repository.UpdatedBy), Parameter: nameof(Repository.UpdatedBy)),
                (Rule: await IsInvalidAsync(repository.UpdatedDate), Parameter: nameof(Repository.UpdatedDate)),
                (Rule: await IsInvalidLengthAsync(repository.Name, 255), Parameter: nameof(Repository.Name)),
                (Rule: await IsInvalidLengthAsync(repository.Owner, 255), Parameter: nameof(Repository.Owner)),

                (Rule: await IsInvalidLengthAsync(repository.ExternalId, 255),
                    Parameter: nameof(Repository.ExternalId)));
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
            Message = $"Text exceeds max length of {maxLength} characters"
        };

        private static async ValueTask<bool> IsExceedingLengthAsync(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

        private static void ValidateRepositoryIsNotNull(Repository repository)
        {
            if (repository is null)
            {
                throw new NullRepositoryException(message: "Repository is null");
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidRepositoryException =
                new InvalidRepositoryException(
                message: "Repository is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidRepositoryException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }

            }

            invalidRepositoryException.ThrowIfContainsErrors();
        }
    }
}