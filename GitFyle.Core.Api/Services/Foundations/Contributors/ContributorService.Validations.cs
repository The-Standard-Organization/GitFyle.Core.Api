// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;

namespace GitFyle.Core.Api.Services.Foundations.Contributors
{
    internal partial class ContributorService
    {
        private async ValueTask ValidateContributorOnAddAsync(Contributor contributor)
        {
            ValidateContributorIsNotNull(contributor);

        }

        private static void ValidateContributorIsNotNull(Contributor contributor)
        {
            if (contributor is null)
            {
                throw new NullContributorException(message: "Contributor is null");
            }
        }
    }
}