﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Xeptions;

namespace Validations
{
    public static class ValidationEngine
    {
        public static async ValueTask Validate<T>(
            string message,
            params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            var invalidDataException = (T)Activator.CreateInstance(typeof(T), message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (String.IsNullOrWhiteSpace(parameter))
                {
                    throw new InvalidOperationException("Parameter is invalid");
                }

                if (rule == null)
                {
                    throw new InvalidOperationException("Rule in invalid");
                }

                bool ruleCondition = SafeGetDynamicProperty<bool>(rule, "Condition");
                string ruleMessage = SafeGetDynamicProperty<string>(rule, "Message");

                if (String.IsNullOrWhiteSpace(ruleMessage))
                {
                    throw new InvalidOperationException("Message in invalid");
                }

                //if (invalidDataException.Data.Count < 500)
                //{
                if (ruleCondition)
                {
                    invalidDataException.UpsertDataList(
                        key: parameter,
                        value: ruleMessage);
                }
                //}
            }

            //if (invalidDataException.Data.Count < 500)
            //{
            invalidDataException?.ThrowIfContainsErrors();
            //}

            await ValueTask.CompletedTask;
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