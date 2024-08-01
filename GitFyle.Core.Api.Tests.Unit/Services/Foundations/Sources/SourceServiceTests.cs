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
    }
}
