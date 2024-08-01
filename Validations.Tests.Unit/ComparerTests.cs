// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using Tynamix.ObjectFiller;

namespace Validations.Tests.Unit
{
    public partial class ComparerTests
    {
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
    }
}
