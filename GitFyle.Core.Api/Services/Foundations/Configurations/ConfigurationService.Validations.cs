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
        private void ValidateConfigurationOnAdd(Configuration configuration)
        {
            ValidateConfigurationIsNotNull(configuration);

            Validate(
                (Rule: IsInvalid(configuration.Id), Parameter: nameof(configuration.Id)),
                (Rule: IsInvalid(configuration.Name), Parameter: nameof(configuration.Name)),
                (Rule: IsInvalid(configuration.Type), Parameter: nameof(configuration.Type)),
                (Rule: IsInvalid(configuration.Value), Parameter: nameof(configuration.Value)),
                (Rule: IsInvalid(configuration.CreatedBy), Parameter: nameof(configuration.CreatedBy)),
                (Rule: IsInvalid(configuration.UpdatedBy), Parameter: nameof(configuration.UpdatedBy)),
                (Rule: IsInvalid(configuration.CreatedDate), Parameter: nameof(configuration.CreatedDate)),
                (Rule: IsInvalid(configuration.UpdatedDate), Parameter: nameof(configuration.UpdatedDate))
                );
        }

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

        private void ValidateConfigurationIsNotNull(Configuration configuration) 
        {
            if (configuration == null)
            {
                throw new NullConfigurationException(message: "Configuration is null");
            }
        }
    }
}
