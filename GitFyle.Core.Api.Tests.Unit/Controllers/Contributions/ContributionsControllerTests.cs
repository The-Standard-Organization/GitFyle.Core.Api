// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Services.Foundations.Contributions;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Contributions
{
    public partial class ContributionsControllerTests : RESTFulController
    {
        private readonly Mock<IContributionService> contributionServiceMock;
        private readonly ContributionsController contributionsController;

        public ContributionsControllerTests()
        {
            this.contributionServiceMock = new Mock<IContributionService>();

            this.contributionsController = new ContributionsController(
                contributionService: this.contributionServiceMock.Object);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IQueryable<Contribution> CreateRandomnContributions() =>
            CreateRandomContributions().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<Contribution> CreateRandomContributions()
        {
            var filler = new Filler<Contribution>();

            filler.Setup()
                .OnProperty(contribution =>
                    contribution.ContributionType).IgnoreIt()

                .OnProperty(contribution =>
                    contribution.Contributor).IgnoreIt()

                .OnProperty(contribution =>
                    contribution.Repository).IgnoreIt()

                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset);



            return filler;
        }
    }
}
