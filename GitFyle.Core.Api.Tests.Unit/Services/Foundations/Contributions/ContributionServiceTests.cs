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
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Services.Foundations.Contributions;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Contributions
{
    public partial class ContributionServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ContributionService contributionService;

        public ContributionServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.contributionService = new ContributionService(
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

        private static Contribution CreateRandomContribution() =>
            CreateRandomContribution(dateTimeOffset: GetRandomDateTimeOffset());

        private static IQueryable<Contribution> CreateRandomContributions()
        {
            return CreateContributionFiller(GetRandomDateTimeOffset())
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static Contribution CreateRandomContribution(DateTimeOffset dateTimeOffset) =>
            CreateContributionFiller(dateTimeOffset).Create();

        private static Filler<Contribution> CreateContributionFiller(DateTimeOffset dateTimeOffset)
        {
            string user = Guid.NewGuid().ToString();
            var filler = new Filler<Contribution>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(contribution => contribution.CreatedBy).Use(user)
                .OnProperty(contribution => contribution.UpdatedBy).Use(user)
                .OnProperty(contribution => contribution.ContributionType).IgnoreIt()
                .OnProperty(contribution => contribution.Contributor).IgnoreIt()
                .OnProperty(repository => repository.Repository).IgnoreIt();

            return filler;
        }
    }
}
