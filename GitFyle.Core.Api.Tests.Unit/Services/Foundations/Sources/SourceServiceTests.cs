// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly SourceService sourceService;

        public SourceServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.sourceService = new SourceService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        private SqlException CreateSqlException()
        {
            return (SqlException)RuntimeHelpers.GetUninitializedObject(
                type: typeof(SqlException));
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            return new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length)
                .GetValue();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static Source CreateRandomSource() =>
            CreateRandomSource(dateTimeOffset: GetRandomDateTimeOffset());

        private static Source CreateRandomSource(DateTimeOffset dateTimeOffset) =>
            CreateSourceFiller(dateTimeOffset).Create();

        private static IQueryable<Source> CreateRandomSources()
        {
            return CreateSourceFiller(GetRandomDateTimeOffset())
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static Filler<Source> CreateSourceFiller(DateTimeOffset dateTimeOffset)
        {
            string someUser = Guid.NewGuid().ToString();
            var filler = new Filler<Source>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(address => address.Url).Use(new RandomUrl().GetValue())
                .OnProperty(someUser => someUser.CreatedBy).Use(someUser)
                .OnProperty(someUser => someUser.UpdatedBy).Use(someUser)
                .OnProperty(source => source.Repositories).IgnoreIt()
                .OnProperty(source => source.Contributors).IgnoreIt();

            return filler;
        }
    }
}