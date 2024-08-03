// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Services.Foundations.Sources;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit.Abstractions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<IValidationBroker> validationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly SourceService sourceService;
        private readonly ICompareLogic compareLogic;
        private readonly ITestOutputHelper output;

        public SourceServiceTests(ITestOutputHelper output)
        {
            this.output = output;
            this.compareLogic = new CompareLogic();
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.validationBrokerMock = new Mock<IValidationBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.sourceService = new SourceService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                validationBroker: this.validationBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private Expression<Func<(dynamic Rule, string Parameter)[], bool>> SameValidationCriteriaAs(
            (dynamic Rule, string Parameter)[] expectedValidationCriteria)
        {
            return actualValidationCriteria =>
                this.compareLogic.Compare(expectedValidationCriteria, actualValidationCriteria)
                    .AreEqual;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        public Expression<Func<(dynamic Rule, string Parameter)[], bool>> SameRulesAs(
            (dynamic Rule, string Parameter)[] expectedRules) =>
                actualRules => IsSameRulesAs(actualRules, expectedRules);

        public bool IsSameRulesAs(
            (dynamic Rule, string Parameter)[] actualRules,
            (dynamic Rule, string Parameter)[] expectedRules)
        {
            var comparisonResult = this.compareLogic.Compare(actualRules, expectedRules);

            if (!comparisonResult.AreEqual)
                this.output.WriteLine(comparisonResult.DifferencesString);

            return comparisonResult.AreEqual;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Source CreateRandomSource() =>
            CreateRandomSource(dateTimeOffset: GetRandomDateTimeOffset());

        private static Source CreateRandomSource(DateTimeOffset dateTimeOffset) =>
            CreateSourceFiller(dateTimeOffset).Create();

        private static Filler<Source> CreateSourceFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<Source>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(source => source.Name).Use(() => GetRandomString())
                .OnProperty(address => address.Url).Use(new RandomUrl().GetValue())
                .OnProperty(address => address.CreatedBy).Use(user)
                .OnProperty(address => address.UpdatedBy).Use(user)
                .OnProperty(source => source.Repositories).IgnoreIt()
                .OnProperty(source => source.Contributors).IgnoreIt();

            return filler;
        }

        private (dynamic Rule, string Parameter)[] CreateValidationCriteria(
            Source source,
            bool idCondition = false,
            bool nameCondition = false,
            bool urlCondition = false,
            bool nameLengthCondition = false,
            bool createdByCondition = false,
            bool updatedByCondition = false,
            bool createdDateCondition = false,
            bool updatedDateCondition = false,
            bool updatedByMatchCreatedByCondition = false,
            bool updatedDateMatchCreatedDateCondition = false)
        {
            return new (dynamic Rule, string Parameter)[]
            {
                (
                    Rule: new
                    {
                        Condition = idCondition,
                        Message = "Id is invalid",
                        Values = new object[] { source.Id }
                    },
                    Parameter: nameof(Source.Id)
                ),
                (
                    Rule: new
                    {
                        Condition = nameCondition,
                        Message = "Text is invalid",
                        Values = new object[] { source.Name }
                    },
                    Parameter: nameof(Source.Name)
                ),
                (
                    Rule: new
                    {
                        Condition = urlCondition,
                        Message = "Url is invalid",
                        Values = new object[] { source.Url }
                    },
                    Parameter: nameof(Source.Url)
                ),
                (
                    Rule: new
                    {
                        Condition = nameLengthCondition,
                        Message = "Text exceed max length of 255 characters",
                        Values = new object[] { source.Name, 255 }
                    },
                    Parameter: nameof(Source.Name)
                ),
                (
                    Rule: new
                    {
                        Condition = createdByCondition,
                        Message = "Text is invalid",
                        Values = new object[] { source.CreatedBy }
                    },
                    Parameter: nameof(Source.CreatedBy)
                ),
                (
                    Rule: new
                    {
                        Condition = updatedByCondition,
                        Message = "Text is invalid",
                        Values = new object[] { source.UpdatedBy }
                    },
                    Parameter: nameof(Source.UpdatedBy)
                ),
                (
                    Rule: new
                    {
                        Condition = createdDateCondition,
                        Message = "Date is invalid",
                        Values = new object[] { source.CreatedDate }
                    },
                    Parameter: nameof(Source.CreatedDate)
                ),
                (
                    Rule: new
                    {
                        Condition = updatedDateCondition,
                        Message = "Date is invalid",
                        Values = new object[] { source.UpdatedDate }
                    },
                    Parameter: nameof(Source.UpdatedDate)
                ),
                (
                    Rule: new
                    {
                        Condition = updatedByMatchCreatedByCondition,
                        Message = $"Text is not the same as {nameof(Source.CreatedBy)}",
                        Values = new object[] { source.UpdatedBy, source.CreatedBy, nameof(Source.CreatedBy) }
                    },
                    Parameter: nameof(Source.UpdatedBy)
                ),
                (
                    Rule: new
                    {
                        Condition = updatedDateMatchCreatedDateCondition,
                        Message = $"Date is not the same as {nameof(Source.CreatedDate)}",
                        Values = new object[] { source.UpdatedDate, source.CreatedDate, nameof(Source.CreatedDate) }
                    },
                    Parameter: nameof(Source.UpdatedDate)
                )
            };
        }
    }
}
