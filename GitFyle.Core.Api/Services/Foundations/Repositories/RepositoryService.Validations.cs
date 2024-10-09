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
                (Rule: IsInvalid(repository.Id), Parameter: nameof(Repository.Id)),
                (Rule: IsInvalid(repository.Name), Parameter: nameof(Repository.Name)),
                (Rule: IsInvalid(repository.Owner), Parameter: nameof(Repository.Owner)),
                (Rule: IsInvalid(repository.ExternalId), Parameter: nameof(Repository.ExternalId)),
                (Rule: IsInvalid(repository.SourceId), Parameter: nameof(Repository.SourceId)),
                (Rule: IsInvalid(repository.Token), Parameter: nameof(Repository.Token)),
                (Rule: IsInvalid(repository.TokenExpireAt), Parameter: nameof(Repository.TokenExpireAt)),
                (Rule: IsInvalid(repository.Description), Parameter: nameof(Repository.Description)),

                (Rule: IsInvalid(repository.ExternalCreatedAt),
                    Parameter: nameof(Repository.ExternalCreatedAt)),

                (Rule: IsInvalid(repository.ExternalUpdatedAt),
                    Parameter: nameof(Repository.ExternalUpdatedAt)),

                (Rule: IsInvalid(repository.CreatedBy), Parameter: nameof(Repository.CreatedBy)),
                (Rule: IsInvalid(repository.CreatedDate), Parameter: nameof(Repository.CreatedDate)),
                (Rule: IsInvalid(repository.UpdatedBy), Parameter: nameof(Repository.UpdatedBy)),
                (Rule: IsInvalid(repository.UpdatedDate), Parameter: nameof(Repository.UpdatedDate)),
                (Rule: IsInvalidLength(repository.Name, 255), Parameter: nameof(Repository.Name)),
                (Rule: IsInvalidLength(repository.Owner, 255), Parameter: nameof(Repository.Owner)),

                (Rule: IsInvalidLength(repository.ExternalId, 255),
                    Parameter: nameof(Repository.ExternalId)),

                (Rule: IsNotSame(
                    first: repository.UpdatedBy,
                    second: repository.CreatedBy,
                    secondName: nameof(Repository.CreatedBy)),

                Parameter: nameof(Repository.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: repository.UpdatedDate,
                    secondDate: repository.CreatedDate,
                    secondDateName: nameof(Repository.CreatedDate)),

                Parameter: nameof(Repository.UpdatedDate)),

                (Rule: await IsNotRecentAsync(repository.CreatedDate), Parameter: nameof(Repository.CreatedDate)));
        }

        private async ValueTask ValidateRepositoryOnModifyAsync(Repository repository)
        {
            ValidateRepositoryIsNotNull(repository);

            Validate(
                (Rule: IsInvalid(repository.Id), Parameter: nameof(Repository.Id)),
                (Rule: IsInvalid(repository.Name), Parameter: nameof(Repository.Name)),
                (Rule: IsInvalid(repository.Owner), Parameter: nameof(Repository.Owner)),
                (Rule: IsInvalid(repository.ExternalId), Parameter: nameof(Repository.ExternalId)),
                (Rule: IsInvalid(repository.SourceId), Parameter: nameof(Repository.SourceId)),
                (Rule: IsInvalid(repository.Token), Parameter: nameof(Repository.Token)),
                (Rule: IsInvalid(repository.TokenExpireAt), Parameter: nameof(Repository.TokenExpireAt)),
                (Rule: IsInvalid(repository.Description), Parameter: nameof(Repository.Description)),

                (Rule: IsInvalid(repository.ExternalCreatedAt),
                    Parameter: nameof(Repository.ExternalCreatedAt)),

                (Rule: IsInvalid(repository.ExternalUpdatedAt),
                    Parameter: nameof(Repository.ExternalUpdatedAt)),

                (Rule: IsInvalid(repository.CreatedBy), Parameter: nameof(Repository.CreatedBy)),
                (Rule: IsInvalid(repository.CreatedDate), Parameter: nameof(Repository.CreatedDate)),
                (Rule: IsInvalid(repository.UpdatedBy), Parameter: nameof(Repository.UpdatedBy)),
                (Rule: IsInvalid(repository.UpdatedDate), Parameter: nameof(Repository.UpdatedDate)),
                (Rule: IsInvalidLength(repository.Name, 255), Parameter: nameof(Repository.Name)),
                (Rule: IsInvalidLength(repository.Owner, 255),Parameter: nameof(Repository.Owner)),

                (Rule: IsInvalidLength(repository.ExternalId, 255),
                    Parameter: nameof(Repository.ExternalId)),

                (Rule: IsSame(
                    firstDate: repository.UpdatedDate,
                    secondDate: repository.CreatedDate,
                    secondDateName: nameof(Repository.CreatedDate)),

                Parameter: nameof(Repository.UpdatedDate)),

                (Rule: await IsNotRecentAsync(repository.UpdatedDate),
                    Parameter: nameof(Repository.UpdatedDate)));
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

        private static void ValidateRepositoryId(Guid repositoryId) =>
            Validate((Rule: IsInvalid(repositoryId), Parameter: nameof(Repository.Id)));

        private static void ValidateRepositoryIsNotNull(Repository repository)
        {
            if (repository is null)
            {
                throw new NullRepositoryException(message: "Repository is null");
            }
        }

        private static void ValidateStorageRepository(Repository maybeRepository, Guid repositoryId)
        {
            if (maybeRepository is null)
            {
                throw new NotFoundRepositoryException(
                    message: $"Repository not found with id: {repositoryId}");
            }
        }

        private static void ValidateAgainstStorageRepositoryOnModify(
          Repository inputRepository, Repository storageRepository)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputRepository.CreatedBy,
                    second: storageRepository.CreatedBy,
                    secondName: nameof(Repository.CreatedBy)),

                Parameter: nameof(Repository.CreatedBy)),

                (Rule: IsNotSame(
                    firstDate: inputRepository.CreatedDate,
                    secondDate: storageRepository.CreatedDate,
                    secondDateName: nameof(Repository.CreatedDate)),

                Parameter: nameof(Repository.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputRepository.UpdatedDate,
                    secondDate: storageRepository.UpdatedDate,
                    secondDateName: nameof(Repository.UpdatedDate)),

                Parameter: nameof(Repository.UpdatedDate)));
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