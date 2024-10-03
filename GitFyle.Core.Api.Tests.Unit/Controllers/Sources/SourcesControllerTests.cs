// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Sources
{
    public partial class SourcesControllerTests : RESTFulController
    {
        private readonly Mock<ISourceService> sourceServiceMock;
        private readonly SourcesController sourcesController;

        public SourcesControllerTests()
        {
            this.sourceServiceMock = new Mock<ISourceService>();

            this.sourcesController = new SourcesController(
                sourceService: this.sourceServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new SourceValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new SourceDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new SourceDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new SourceServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static IQueryable<Source> CreateRandomSources() =>
            CreateSourceFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static Source CreateRandomSource() =>
            CreateSourceFiller().Create();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Filler<Source> CreateSourceFiller()
        {
            var filler = new Filler<Source>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnProperty(source => source.Contributors).IgnoreIt()
                .OnProperty(source => source.Repositories).IgnoreIt();

            return filler;
        }
    }
}
