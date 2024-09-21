// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Moq;
using Tynamix.ObjectFiller;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Sources
{
    public partial class SourcesControllerTests
    {
        private readonly Mock<ISourceService> sourceServiceMock;
        private readonly SourcesController sourcesController;

        public SourcesControllerTests()
        {
            this.sourceServiceMock = new Mock<ISourceService>();

            this.sourcesController = new SourcesController(
                sourceService: this.sourceServiceMock.Object);
        }

        private static Source CreateRandomSource() =>
            CreateSourceFiller().Create();

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
