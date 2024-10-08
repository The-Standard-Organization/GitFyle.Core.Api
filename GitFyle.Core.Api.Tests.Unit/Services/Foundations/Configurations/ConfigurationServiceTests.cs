﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Services.Foundations.Configurations;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Configurations
{
    public partial class ConfigurationServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> datetimeBrokerMock;
        private readonly ConfigurationService configurationService;

        public ConfigurationServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.datetimeBrokerMock = new Mock<IDateTimeBroker>();

            this.configurationService = new ConfigurationService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.datetimeBrokerMock.Object);
        }

        private static Configuration CreateRandomModifyConfiguration(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInThePast = CreateRandomNegativeNumber();
            Configuration randomConfiguration = CreateRandomConfiguration(dateTimeOffset);

            randomConfiguration.CreatedDate = dateTimeOffset.AddDays(randomDaysInThePast);

            return randomConfiguration;
        }

        private static int CreateRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

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

        private static string GetRandomStringWithLengthOf(int length)
        {
            return new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length)
                .GetValue();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Configuration CreateRandomConfiguration(DateTimeOffset dateTimeOffset) =>
            CreateConfigurationFiller(dateTimeOffset).Create();

        private static Configuration CreateRandomConfiguration() =>
            CreateConfigurationFiller(GetRandomDateTimeOffset()).Create();

        private static IQueryable<Configuration> CreateRandomConfigurations()
        {
            return CreateConfigurationFiller(GetRandomDateTimeOffset())
                .Create(GetRandomNumber())
                .AsQueryable();
        }

        private static Filler<Configuration> CreateConfigurationFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<Configuration>();
            string user = Guid.NewGuid().ToString();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(configuration => configuration.CreatedBy).Use(user)
                .OnProperty(configuration => configuration.UpdatedBy).Use(user);

            return filler;
        }
    }
}