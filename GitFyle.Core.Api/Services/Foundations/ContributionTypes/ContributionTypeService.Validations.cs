﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.ContributionTypes
{
    internal partial class ContributionTypeService
    {
        private async ValueTask ValidateContributionTypeOnAddAsync(ContributionType contributionType)
        {
            ValidateContributionTypeIsNotNull(contributionType);

            Validate(
                (Rule: IsInvalid(contributionType.Id),
                    Parameter: nameof(ContributionType.Id)),

                (Rule: IsInvalid(contributionType.Name),
                    Parameter: nameof(ContributionType.Name)),

                (Rule: IsInvalid(contributionType.CreatedBy),
                    Parameter: nameof(ContributionType.CreatedBy)),

                (Rule: IsInvalid(contributionType.CreatedDate),
                    Parameter: nameof(ContributionType.CreatedDate)),

                (Rule: IsInvalid(contributionType.UpdatedBy),
                    Parameter: nameof(ContributionType.UpdatedBy)),

                (Rule: IsInvalid(contributionType.UpdatedDate),
                    Parameter: nameof(ContributionType.UpdatedDate)));
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

        private static void ValidateContributionTypeId(Guid contributionTypeId) =>
            Validate((Rule: IsInvalid(contributionTypeId), Parameter: nameof(ContributionType.Id)));

        private static void ValidateContributionTypeIsNotNull(ContributionType contributionType)
        {
            if (contributionType is null)
            {
                throw new NullContributionTypeException(message: "ContributionType is null");
            }
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidContributionTypeException =
                new InvalidContributionTypeException(
                message: "ContributionType is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidContributionTypeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidContributionTypeException.ThrowIfContainsErrors();
        }
    }
}