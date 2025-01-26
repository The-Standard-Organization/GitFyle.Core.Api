// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Configurations
{
    internal partial class ConfigurationService
    {
        private async ValueTask ValidateConfigurationOnAddAsync(Configuration configuration)
        {
            ValidateConfigurationIsNotNull(configuration);

            Validate(
                (Rule: IsInvalid(configuration.Id), Parameter: nameof(configuration.Id)),
                (Rule: IsInvalid(configuration.Name), Parameter: nameof(configuration.Name)),
                (Rule: IsInvalid(configuration.Value), Parameter: nameof(configuration.Value)),
                (Rule: IsInvalid(configuration.CreatedBy), Parameter: nameof(configuration.CreatedBy)),
                (Rule: IsInvalid(configuration.CreatedDate), Parameter: nameof(configuration.CreatedDate)),
                (Rule: IsInvalid(configuration.UpdatedBy), Parameter: nameof(configuration.UpdatedBy)),
                (Rule: IsInvalid(configuration.UpdatedDate), Parameter: nameof(configuration.UpdatedDate)),
                (Rule: IsInvalidLength(configuration.Name, 450), Parameter: nameof(Configuration.Name)),

                (Rule: IsNotSame(
                    first: configuration.CreatedBy,
                    second: configuration.UpdatedBy,
                    secondName: nameof(configuration.CreatedBy)),

                    Parameter: nameof(configuration.UpdatedBy)),

                (Rule: IsNotSame(
                    firstDate: configuration.CreatedDate,
                    secondDate: configuration.UpdatedDate,
                    secondDateName: nameof(configuration.CreatedDate)),

                    Parameter: nameof(configuration.UpdatedDate)),

                (Rule: await IsNotRecentAsync(configuration.CreatedDate),
                Parameter: nameof(configuration.CreatedDate)));
        }

        private async ValueTask ValidateConfigurationOnModify(Configuration configuration)
        {
            ValidateConfigurationIsNotNull(configuration);

            Validate(
                (Rule: IsInvalid(configuration.Id), Parameter: nameof(configuration.Id)),
                (Rule: IsInvalid(configuration.Name), Parameter: nameof(configuration.Name)),
                (Rule: IsInvalid(configuration.Value), Parameter: nameof(configuration.Value)),
                (Rule: IsInvalid(configuration.CreatedBy), Parameter: nameof(configuration.CreatedBy)),
                (Rule: IsInvalid(configuration.CreatedDate), Parameter: nameof(configuration.CreatedDate)),
                (Rule: IsInvalid(configuration.UpdatedBy), Parameter: nameof(configuration.UpdatedBy)),
                (Rule: IsInvalid(configuration.UpdatedDate), Parameter: nameof(configuration.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: configuration.UpdatedDate,
                    secondDate: configuration.CreatedDate,
                    secondDateName: nameof(Configuration.CreatedDate)),
                    Parameter: nameof(configuration.UpdatedDate)),

                (Rule: IsInvalidLength(configuration.Name, 450), Parameter: nameof(Configuration.Name)),

                (Rule: await IsNotRecentAsync(configuration.UpdatedDate),
                Parameter: nameof(configuration.UpdatedDate)));
        }
        private static void ValidateConfigurationId(Guid configurationId) =>
            Validate((Rule: IsInvalid(configurationId), Parameter: nameof(Configuration.Id)));

        private static void ValidateStorageConfiguration(Configuration configuration, Guid id)
        {
            if (configuration is null)
            {
                throw new NotFoundConfigurationException(
                    message: $"Configuration not found with id: {id}");
            }
        }

        private static void ValidateAgainstStorageConfigurationOnModify(
            Configuration inputConfiguration, Configuration storageConfiguration)
        {
            Validate(
                (Rule: IsNotSame(
                    first: inputConfiguration.CreatedBy,
                    second: storageConfiguration.CreatedBy,
                    secondName: nameof(Configuration.CreatedBy)),

                Parameter: nameof(Configuration.CreatedBy)),

                (Rule: IsNotSame(
                    firstDate: inputConfiguration.CreatedDate,
                    secondDate: storageConfiguration.CreatedDate,
                    secondDateName: nameof(Configuration.CreatedDate)),

                Parameter: nameof(Configuration.CreatedDate)),

                (Rule: IsSame(
                    firstDate: inputConfiguration.UpdatedDate,
                    secondDate: storageConfiguration.UpdatedDate,
                    secondDateName: nameof(Configuration.UpdatedDate)),

                Parameter: nameof(Configuration.UpdatedDate)));
        }

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is same as {secondDateName}"
            };

        private static dynamic IsInvalidLength(string text, int maxLength) => new
        {
            Condition = IsExceedingLength(text, maxLength),
            Message = $"Text exceed max length of {maxLength} characters"
        };

        private static bool IsExceedingLength(string text, int maxLength) =>
            (text ?? string.Empty).Length > maxLength;

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

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            string first,
            string second,
            string secondName) => new
            {
                Condition = first != second,
                Message = $"Text is not same as {secondName}"
            };

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid."
        };

        private static dynamic IsInvalid(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is required."
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is invalid."
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidConfigurationException = new InvalidConfigurationException(
                message: "Configuration is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidConfigurationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidConfigurationException.ThrowIfContainsErrors();
        }

        private static void ValidateConfigurationIsNotNull(Configuration configuration)
        {
            if (configuration is null)
            {
                throw new NullConfigurationException(message: "Configuration is null");
            }
        }
    }
}
