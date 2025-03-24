// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        private readonly ContributionTypesController contributionTypesController;

        public ContributionTypesControllerTests()
        {
            this.contributionTypeServiceMock = new Mock<IContributionTypeService>();

            this.contributionTypesController = new ContributionTypesController(
                contributionTypeService: this.contributionTypeServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            return new TheoryData<Xeption>
            {
                new ContributionTypeValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new ContributionTypeDependencyValidationException(
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
                new ContributionTypeDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new ContributionTypeServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static ContributionType CreateRandomContributionType() =>
            CreateContributionTypeFiller().Create();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Dictionary<string, string[]> GetRandomDictionaryData()
        {
            var filler = new Filler<Dictionary<string, string[]>>();

            filler.Setup()
                .DictionaryItemCount(maxCount: 10);

            return filler.Create();
        }

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
