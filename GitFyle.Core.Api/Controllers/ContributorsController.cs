// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributors;
using GitFyle.Core.Api.Models.Foundations.Contributors.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Contributors;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/contributors")]
    public class ContributorsController : RESTFulController
    {
        private readonly IContributorService contributorService;

        public ContributorsController(IContributorService contributorService) =>
            this.contributorService = contributorService;

        [HttpPost]
        public async ValueTask<ActionResult<Contributor>> PostContributorAsync(Contributor contributor)
        {
            try
            {
                Contributor addedContributor =
                    await this.contributorService.AddContributorAsync(contributor);

                return Created(addedContributor);
            }
            catch (ContributorValidationException contributorValidationException)
            {
                return BadRequest(contributorValidationException.InnerException);
            }
            catch (ContributorDependencyValidationException contributorDependencyValidationException)
                when (contributorDependencyValidationException.InnerException is InvalidReferenceContributorException)
            {
                return FailedDependency(contributorDependencyValidationException.InnerException);
            }
            catch (ContributorDependencyValidationException contributorDependencyValidationException)
                when (contributorDependencyValidationException.InnerException is AlreadyExistsContributorException)
            {
                return Conflict(contributorDependencyValidationException.InnerException);
            }
            catch (ContributorDependencyValidationException contributorDependencyValidationException)
            {
                return BadRequest(contributorDependencyValidationException.InnerException);
            }
            catch (ContributorDependencyException contributorDependencyException)
            {
                return InternalServerError(contributorDependencyException);
            }
            catch (ContributorServiceException contributorServiceException)
            {
                return InternalServerError(contributorServiceException);
            }
        }
    }
}
