// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Validations.Models;
using Xunit.Abstractions;

namespace Validations
{
    public static partial class Comparer
    {
        /// <summary>
        /// Compares two arrays and writes a failure message to the test explorer if they are not the same.
        /// </summary>
        /// <param name="expectedRules">The expected rules</param>
        /// <param name="testOutputHelper">The helper to write the failure message to the test explorer</param>
        /// <param name="reference">The reference to use that will link the failure message to your code</param>
        /// <returns>An expression to compare the actual rules with the expected rules</returns>
        public static Expression<Func<(dynamic Rule, string Parameter)[], bool>> SameRulesAs(
            (dynamic Rule, string Parameter)[] expectedRules) =>
                actualRules => SameExceptionAs(actualRules, expectedRules);

        /// <summary>
        /// Compares two arrays and writes a failure message to the test explorer if they are not the same.
        /// </summary>
        /// <param name="expectedRules">The expected rules</param>
        /// <param name="testOutputHelper">The helper to write the failure message to the test explorer</param>
        /// <param name="reference">The reference to use that will link the failure message to your code</param>
        /// <returns>An expression to compare the actual rules with the expected rules</returns>
        public static Expression<Func<(dynamic Rule, string Parameter)[], bool>> SameRulesAs(
            (dynamic Rule, string Parameter)[] expectedRules,
            ITestOutputHelper testOutputHelper,
            string reference = "") =>
                actualRules => SameExceptionAs(actualRules, expectedRules, testOutputHelper, reference);



        public static bool SameExceptionAs(
            this (dynamic Rule, string Parameter)[] rules,
            (dynamic Rule, string Parameter)[] otherRules) =>
                IsSameRulesAs(rules, otherRules, out string message);

        public static bool SameExceptionAs(
            this (dynamic Rule, string Parameter)[] rules,
            (dynamic Rule, string Parameter)[] otherRules,
            ITestOutputHelper testOutputHelper,
            string reference = "") =>
                IsSameRulesAs(rules, otherRules, testOutputHelper, reference);

        internal static bool IsSameRulesAs(
            (dynamic Rule, string Parameter)[] actualRules,
            (dynamic Rule, string Parameter)[] expectedRules,
            ITestOutputHelper testOutputHelper,
            string reference = "")

        {
            string message;
            bool result = IsSameRulesAs(actualRules, expectedRules, out message);
            testOutputHelper.WriteLine($"{reference}: {message}");

            return result;
        }

        public static bool IsSameRulesAs(
            (dynamic Rule, string Parameter)[] actualRules,
            (dynamic Rule, string Parameter)[] expectedRules,
            out string message)
        {
            StringBuilder error = new StringBuilder();

            if (actualRules is null && expectedRules is null)
            {
                message = string.Empty;
                return true;
            }

            if (actualRules is null || expectedRules is null)
            {
                error.AppendLine(
                    $"Expected actual rules to be \"{expectedRules?.Count() ?? 0}\", " +
                    $"but found \"{actualRules?.Count() ?? 0}\"");
            }

            var actualRuleList = actualRules.ToList();
            var expectedRuleList = expectedRules.ToList();

            List<ValidationCriteria> actualValidationCriteria = new List<ValidationCriteria>();
            List<ValidationCriteria> expectedValidationCriteria = new List<ValidationCriteria>();

            foreach ((dynamic rule, string parameter) in actualRuleList)
            {
                bool ruleCondition = SafeGetDynamicProperty<bool>(rule, "Condition");
                string ruleMessage = SafeGetDynamicProperty<string>(rule, "Message");

                actualValidationCriteria.Add(new ValidationCriteria
                {
                    Parameter = parameter,
                    Rule = new Rule
                    {
                        Message = ruleMessage,
                        Condition = ruleCondition
                    }
                });
            }

            foreach ((dynamic rule, string parameter) in expectedRuleList)
            {
                bool ruleCondition = SafeGetDynamicProperty<bool>(rule, "Condition");
                string ruleMessage = SafeGetDynamicProperty<string>(rule, "Message");

                expectedValidationCriteria.Add(new ValidationCriteria
                {
                    Parameter = parameter,
                    Rule = new Rule
                    {
                        Message = ruleMessage,
                        Condition = ruleCondition
                    }
                });
            }

            var missingRules = new List<ValidationCriteria>();
            var extraRules = new List<ValidationCriteria>();

            foreach (ValidationCriteria expectedRule in expectedValidationCriteria)
            {
                var matchingRule = actualValidationCriteria.FirstOrDefault(rule =>
                    rule.Parameter == expectedRule.Parameter
                    && rule.Rule.Message == expectedRule.Rule.Message);

                if (matchingRule == default)
                {
                    missingRules.Add(expectedRule);
                }
                else
                {
                    // Remove the matched rule from the actual list for further checks
                    actualValidationCriteria.Remove(matchingRule);

                    // Check if the conditions are different
                    if (matchingRule.Rule.Condition != expectedRule.Rule.Condition)
                    {
                        error.AppendLine(
                            $"For parameter '{expectedRule.Parameter}', " +
                            $"and message '{expectedRule.Rule.Message}', " +
                            $"expected condition '{expectedRule.Rule.Condition}' " +
                            $"but found '{matchingRule.Rule.Condition}'");
                    }
                }
            }

            // Any remaining rules in actualRuleList are extra rules
            extraRules.AddRange(actualValidationCriteria);

            // Report missing rules
            if (missingRules.Any())
            {
                foreach (var missingRule in missingRules)
                {
                    error.AppendLine(
                        $"Actual rules does not contain a expected rule for parameter '{missingRule.Parameter}' " +
                        $"with message '{missingRule.Rule.Message}' " +
                        $"and condition '{missingRule.Rule.Condition}'");
                }
            }

            // Report extra rules
            if (extraRules.Any())
            {
                foreach (var extraRule in extraRules)
                {
                    error.AppendLine(
                        $"Actual rules contains an unexpected rule for " +
                        $"parameter '{extraRule.Parameter}' " +
                        $"with message '{extraRule.Rule.Message}'");
                }
            }

            message = error.ToString();

            return error.Length == 0;
        }

        private static T SafeGetDynamicProperty<T>(dynamic obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);

            if (propertyInfo != null)
            {
                return (T)propertyInfo.GetValue(obj);
            }

            throw new InvalidOperationException($"Property '{propertyName}' not found on dynamic object.");
        }
    }
}
