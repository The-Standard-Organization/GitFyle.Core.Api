// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Data;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Configurations
{
    internal partial class ConfigurationService
    {
        private async ValueTask ValidateConfigurationOnAdd(Configuration configuration)
        {
            ValidateConfigurationIsNotNull(configuration);

            Validate(
                (Rule: await IsInvalidAsync(configuration.Id), Parameter: nameof(configuration.Id)),
                (Rule: await IsInvalidAsync(configuration.Name), Parameter: nameof(configuration.Name)),
                (Rule: await IsInvalidAsync(configuration.Type), Parameter: nameof(configuration.Type)),
                (Rule: await IsInvalidAsync(configuration.Value), Parameter: nameof(configuration.Value)),
                (Rule: await IsInvalidAsync(configuration.CreatedBy), Parameter: nameof(configuration.CreatedBy)),
                (Rule: await IsInvalidAsync(configuration.UpdatedBy), Parameter: nameof(configuration.UpdatedBy)),
                (Rule: await IsInvalidAsync(configuration.CreatedDate), Parameter: nameof(configuration.CreatedDate)),
                (Rule: await IsInvalidAsync(configuration.UpdatedDate), Parameter: nameof(configuration.UpdatedDate))
                );
        }

        private static async ValueTask<dynamic> IsInvalidAsync(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is invalid."
        };

        private static async ValueTask<dynamic> IsInvalidAsync(string name) => new
        {
            Condition = String.IsNullOrWhiteSpace(name),
            Message = "Text is required."
        };

        private static async ValueTask<dynamic> IsInvalidAsync(DateTimeOffset date) => new
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
