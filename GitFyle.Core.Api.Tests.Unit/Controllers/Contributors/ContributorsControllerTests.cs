// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Contributors;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Contributors
{
    public partial class ContributorsControllerTests : RESTFulController
    {
        private readonly Mock<IContributorService> contributorServiceMock;
        private readonly ContributorsController contributorsController;

        public ContributorsControllerTests()
        {
            this.contributorServiceMock = new Mock<IContributorService>();

            this.contributorsController = new ContributorsController(
                contributorService: this.contributorServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            return new TheoryData<Xeption>
            {
                new ContributorValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new ContributorDependencyValidationException(
                    message: someMessage,
                    innerException: someInnerException,
                    data: someDictionaryData)
            };
        }

        public static TheoryData<Xeption> ServerExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            return new TheoryData<Xeption>
            {
                new ContributorDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new ContributorServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static Dictionary<string, string[]> GetRandomDictionaryData()
        {
            var filler = new Filler<Dictionary<string, string[]>>();

            filler.Setup()
                .DictionaryItemCount(maxCount: 10);

            return filler.Create();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IQueryable<Contributor> CreateRandomnContributors() =>
            CreateContributorFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static Contributor CreateRandomContributor() =>
            CreateContributorFiller().Create();

        private static Filler<Contributor> CreateContributorFiller()
        {
            var filler = new Filler<Contributor>();

            filler.Setup()
                .OnProperty(contributor =>
                    contributor.Contributions).IgnoreIt()

                .OnType<DateTimeOffset>().Use(
                    GetRandomDateTimeOffset);

            return filler;
        }
    }
}
