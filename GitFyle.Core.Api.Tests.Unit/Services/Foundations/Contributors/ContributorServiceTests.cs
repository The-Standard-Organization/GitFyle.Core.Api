// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organizator: A coalitor of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Services.Foundations.Contributors;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributors
{
    public partial class ContributorServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ContributorService contributorService;

        public ContributorServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.contributorService = new ContributorService(
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

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static string GetRandomStringWithLengthOf(int length)
        {
            return new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length)
                .GetValue();
        }

        private static Contributor CreateRandomContributor() =>
            CreateRandomContributor(dateTimeOffset: GetRandomDateTimeOffset());

        private static Contributor CreateRandomContributor(DateTimeOffset dateTimeOffset) =>
            CreateContributorFiller(dateTimeOffset).Create();

        private static Filler<Contributor> CreateContributorFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<Contributor>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(contributor => contributor.CreatedBy).Use(user)
                .OnProperty(contributor => contributor.UpdatedBy).Use(user)
                .OnProperty(contributor => contributor.Source).IgnoreIt()
                .OnProperty(contributor => contributor.Contributions).IgnoreIt();

            return filler;
        }
    }
}
