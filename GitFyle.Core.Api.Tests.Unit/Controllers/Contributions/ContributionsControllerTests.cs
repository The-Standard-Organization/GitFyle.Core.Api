// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Services.Foundations.Contributions;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

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

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new ContributionDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new ContributionServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IQueryable<Contribution> CreateRandomnContributions() =>
            CreateContributionFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static Contribution CreateRandomContribution() =>
            CreateContributionFiller().Create();

        private static Filler<Contribution> CreateContributionFiller()
        {
            var filler = new Filler<Contribution>();

            filler.Setup()
                .OnProperty(contribution =>
                    contribution.ContributionType).IgnoreIt()

                .OnProperty(contribution =>
                    contribution.Contributor).IgnoreIt()

                .OnProperty(contribution =>
                    contribution.Repository).IgnoreIt()

                .OnType<DateTimeOffset>().Use(
                    GetRandomDateTimeOffset);

            return filler;
        }
    }
}
