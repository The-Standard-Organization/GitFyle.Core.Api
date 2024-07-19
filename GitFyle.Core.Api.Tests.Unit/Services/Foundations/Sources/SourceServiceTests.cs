// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit.Abstractions;

namespace GitFyle.Core.Api.Tests.Unit.Services.Foundations.Sources
{
    public partial class SourceServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly SourceService sourceService;
        private readonly ITestOutputHelper output;

        public SourceServiceTests(ITestOutputHelper output)
        {
            this.output = output;
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.sourceService = new SourceService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

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
    }
}
