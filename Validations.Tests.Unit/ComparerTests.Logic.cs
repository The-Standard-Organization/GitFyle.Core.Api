// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;

namespace Validations.Tests.Unit
{
    public partial class ComparerTests
    {

        [Fact]
        public void MatchingRuleSetsShouldReturnTrue()
        {
            // Arrange
            string randomReference = GetRandomString();

            (dynamic Rule, string Parameter)[] expectedRules =
                GetRandomRules(count: GetRandomNumber(), condition: true, message: GetRandomString());

            (dynamic Rule, string Parameter)[] actualRules = expectedRules.DeepClone();

            // Act
            bool result = Validations.Comparer.IsSameRulesAs(
                expectedRules,
                actualRules,
                out string message);

            // Assert
            Assert.True(result);
            Assert.True(string.IsNullOrWhiteSpace(message));
        }

        [Fact]
        public void SameRuleAsNonMatchingRulesShouldReturnFalse()
        {
            // Arrange
            List<(dynamic Rule, string Parameter)> overlappingRules =
                GetRandomRules(count: 2, condition: true, message: GetRandomString()).ToList();

            List<(dynamic Rule, string Parameter)> missingRules =
                GetRandomRules(count: 2, condition: true, message: GetRandomString()).ToList();

            List<(dynamic Rule, string Parameter)> unexpectedRules =
                GetRandomRules(count: 2, condition: true, message: GetRandomString()).ToList();

            List<(dynamic Rule, string Parameter)> expectedRules = new List<(dynamic Rule, string Parameter)>();
            expectedRules.AddRange(overlappingRules.DeepClone());
            expectedRules.AddRange(missingRules.DeepClone());

            List<(dynamic Rule, string Parameter)> actualRules = new List<(dynamic Rule, string Parameter)>();
            actualRules.AddRange(overlappingRules.DeepClone());
            actualRules.AddRange(unexpectedRules.DeepClone());

            // Act
            bool result = Validations.Comparer.IsSameRulesAs(
                expectedRules.ToArray(),
                actualRules.ToArray(),
                out string message);

            // Assert
            Assert.False(result);
            Assert.True(string.IsNullOrWhiteSpace(message) is false);
        }
    }
}
