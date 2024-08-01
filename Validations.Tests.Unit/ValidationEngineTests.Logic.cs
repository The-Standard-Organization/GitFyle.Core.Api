// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xeptions;

namespace Validations.Tests.Unit
{
    public partial class ValidationEngineTests
    {
        [Fact]
        public async Task ValidateShouldNotThrowExceptionIfNoFailingConditionsAsync()
        {
            // Given
            (dynamic Rule, string Parameter)[] validations =
                GetRandomRules(count: GetRandomNumber(), condition: false, message: GetRandomString());

            // When, Then
            await ValidationEngine.Validate<Xeption>("Test message", validations);
        }

        [Fact]
        public async Task ValidateShouldAddDataItemsIfConditionsIsTrueAndThrowExceptionAsync()
        {
            // Given
            (dynamic Rule, string Parameter)[] validations =
                GetRandomRules(count: GetRandomNumber(), condition: true, message: GetRandomString());

            var groupedValidationCriteria = validations.ToList().GroupBy(c => c.Parameter);

            // When
            var exception = await Assert.ThrowsAsync<Xeption>(async () =>
                await ValidationEngine.Validate<Xeption>("Test message", validations));

            // Then
            exception.Should().BeOfType<Xeption>();
            exception.Message.Should().Be("Test message");
            var exceptionDataDictionary = new Dictionary<string, object>();

            foreach (DictionaryEntry entry in exception.Data)
            {
                var stringList = new List<string>();

                if (entry.Value is List<string> items)
                {
                    foreach (var item in items)
                    {
                        stringList.Add(item);
                    }
                }

                exceptionDataDictionary[entry.Key.ToString()] = stringList;
            }

            exceptionDataDictionary.Should().NotBeNull();
            exceptionDataDictionary.Count.Should().Be(groupedValidationCriteria.Count());

            foreach (var group in groupedValidationCriteria)
            {
                exceptionDataDictionary.Should().ContainKey(group.Key);

                foreach (var item in group)
                {
                    ((List<string>)exceptionDataDictionary[group.Key]).Should().Contain(item.Rule.Message);
                }
            }
        }

        [Fact]
        public async Task ValidateShouldAddDataItemsIfConditionsIsTrueAndThrowExceptionOnEveryItemAsync()
        {
            List<(dynamic Rule, string Parameter)> validations = new List<(dynamic Rule, string Parameter)>();

            for (int i = 0; i < 1000; i++)
            {
                // Given
                validations.AddRange(GetRandomRules(count: 1, condition: true, message: $"Test {i + 1}"));

                try
                {
                    // When
                    var exception = await Assert.ThrowsAsync<Xeption>(async () =>
                        await ValidationEngine.Validate<Xeption>($"Test {i + 1}", validations.ToArray()));

                    var groupedValidationCriteria = validations.ToList().GroupBy(c => c.Parameter);

                    // Then
                    exception.Should().BeOfType<Xeption>();
                    exception.Message.Should().Be($"Test {i + 1}");
                    var exceptionDataDictionary = new Dictionary<string, object>();

                    foreach (DictionaryEntry entry in exception.Data)
                    {
                        var stringList = new List<string>();

                        if (entry.Value is List<string> items)
                        {
                            foreach (var item in items)
                            {
                                stringList.Add(item);
                            }
                        }

                        exceptionDataDictionary[entry.Key.ToString()] = stringList;
                    }

                    exceptionDataDictionary.Should().NotBeNull();
                    exceptionDataDictionary.Count.Should().Be(groupedValidationCriteria.Count());

                    foreach (var group in groupedValidationCriteria)
                    {
                        exceptionDataDictionary.Should().ContainKey(group.Key);

                        foreach (var item in group)
                        {
                            ((List<string>)exceptionDataDictionary[group.Key]).Should().Contain(item.Rule.Message);
                        }
                    }


                }
                catch (Exception error)
                {
                    Assert.Fail($"Expected a Xeption to be thrown for Test {i + 1}. {error.Message}");
                }
            }
        }

        [Fact]
        public async Task ValidateShouldAddDataItemsIfConditionsTrueForMultipleKeysSameParameterAsync()
        {
            // Given
            (dynamic Rule, string Parameter)[] validations =
                GetRandomRulesForParameter(
                    count: GetRandomNumber(),
                    condition: true,
                    message: GetRandomString(),
                    parameter: GetRandomString());

            var groupedValidationCriteria = validations.ToList().GroupBy(c => c.Parameter);

            // When
            var exception = await Assert.ThrowsAsync<Xeption>(async () =>
                await ValidationEngine.Validate<Xeption>("Test message", validations));

            // Then
            exception.Should().BeOfType<Xeption>();
            exception.Message.Should().Be("Test message");
            var exceptionDataDictionary = new Dictionary<string, object>();

            foreach (DictionaryEntry entry in exception.Data)
            {
                var stringList = new List<string>();

                if (entry.Value is List<string> items)
                {
                    foreach (var item in items)
                    {
                        stringList.Add(item);
                    }
                }

                exceptionDataDictionary[entry.Key.ToString()] = stringList;
            }

            exceptionDataDictionary.Should().NotBeNull();
            exceptionDataDictionary.Count.Should().Be(groupedValidationCriteria.Count());

            foreach (var group in groupedValidationCriteria)
            {
                exceptionDataDictionary.Should().ContainKey(group.Key);

                foreach (var item in group)
                {
                    ((List<string>)exceptionDataDictionary[group.Key]).Should().Contain(item.Rule.Message);
                }
            }
        }
    }
}
