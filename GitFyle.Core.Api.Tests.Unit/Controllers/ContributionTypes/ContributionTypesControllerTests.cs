// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using GitFyle.Core.Api.Services.Foundations.ContributionTypes;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.ContributionTypes
{
    public partial class ContributionTypesControllerTests : RESTFulController
    {
        private readonly Mock<IContributionTypeService> contributionTypeServiceMock;
        private readonly ContributionTypesController repositoriesController;

        public ContributionTypesControllerTests()
        {
            this.contributionTypeServiceMock = new Mock<IContributionTypeService>();

            this.repositoriesController = new ContributionTypesController(
                contributionTypeService: this.contributionTypeServiceMock.Object);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static ContributionType CreateRandomContributionType() =>
            CreateContributionTypeFiller().Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Filler<ContributionType> CreateContributionTypeFiller()
        {
            var filler = new Filler<ContributionType>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnProperty(contributionType => contributionType.Contributions).IgnoreIt();

            return filler;
        }
    }
}
