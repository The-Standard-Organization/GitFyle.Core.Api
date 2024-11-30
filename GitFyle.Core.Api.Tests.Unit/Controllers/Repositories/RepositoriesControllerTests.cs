// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using GitFyle.Core.Api.Controllers;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Repositories;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;
using Xeptions;

namespace GitFyle.Core.Api.Tests.Unit.Controllers.Repositories
{
    public partial class RepositoriesControllerTests : RESTFulController
    {
        private readonly Mock<IRepositoryService> repositoryServiceMock;
        private readonly RepositoriesController repositoriesController;

        public RepositoriesControllerTests()
        {
            this.repositoryServiceMock = new Mock<IRepositoryService>();

            this.repositoriesController = new RepositoriesController(
                repositoryService: this.repositoryServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();
            var someDictionaryData = GetRandomDictionaryData();

            return new TheoryData<Xeption>
            {
                new RepositoryValidationException(
                    message: someMessage,
                    innerException: someInnerException),

                new RepositoryDependencyValidationException(
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
                new RepositoryDependencyException(
                    message: someMessage,
                    innerException: someInnerException),

                new RepositoryServiceException(
                    message: someMessage,
                    innerException: someInnerException)
            };
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static IQueryable<Repository> CreateRandomRepositories() =>
            CreateRepositoryFiller().Create(count: GetRandomNumber()).AsQueryable();

        private static Repository CreateRandomRepository() =>
            CreateRepositoryFiller().Create();

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

        private static Filler<Repository> CreateRepositoryFiller()
        {
            var filler = new Filler<Repository>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnProperty(repository => repository.Source).IgnoreIt()
                .OnProperty(repository => repository.Contributions).IgnoreIt();

            return filler;
        }
    }
}
