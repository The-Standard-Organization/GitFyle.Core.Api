﻿// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly SourceService sourceService;

        public SourceServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.sourceService = new SourceService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(
            Xeption expectedException)
        {
            return actualException =>
                actualException.SameExceptionAs(expectedException);
        }

        private static Source CreateRandomSource() =>
            CreateSourceFiller().Create();

        private static Filler<Source> CreateSourceFiller()
        {
            var filler = new Filler<Source>();

            filler.Setup()
                .OnProperty(source => source.Repositories).IgnoreIt()
                .OnProperty(source => source.Contributors).IgnoreIt();

            return filler;
        }

        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask MustCompleteWithinTimeout(
            ReturningNothingFunction returningNothingFunction,
            int timeoutMilliseconds)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = Task.Delay(timeoutMilliseconds, cancellationTokenSource.Token);
            var testTask = Task.Run(async () => await returningNothingFunction(), cancellationTokenSource.Token);
            var completedTask = await Task.WhenAny(testTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                cancellationTokenSource.Cancel();
                throw new TimeoutException($"The test exceeded the allowed timeout period of {timeoutMilliseconds}ms.");
            }

            await testTask;
        }
    }
}
