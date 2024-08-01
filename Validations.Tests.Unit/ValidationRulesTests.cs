// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Validations.Tests.Unit
{
    public partial class ValidationRulesTests
    {

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        private static int GetRandomNumber(int min = 2, int max = 10) =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1, wordMinLength: 2, wordMaxLength: GetRandomNumber()).GetValue();

        private static (dynamic Rule, string Parameter)[] GetRandomRules(
            int count,
            bool condition,
            string message)
        {
            return Enumerable.Range(1, count)
                .Select(_ =>
                (
                    Rule: (dynamic)new { Condition = condition, Message = message },
                    Parameter: new MnemonicString(wordCount: 1).GetValue()
                )).ToArray();
        }

        private static (dynamic Rule, string Parameter)[] GetRandomRulesForParameter(
            int count,
            bool condition,
            string message,
            string parameter)
        {
            return Enumerable.Range(1, count)
                .Select(_ =>
                    (
                        Rule: (dynamic)new { Condition = condition, Message = message },
                        Parameter: parameter
                    )).ToArray();
        }
        private T SafeGetDynamicProperty<T>(dynamic obj, string propertyName)
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
