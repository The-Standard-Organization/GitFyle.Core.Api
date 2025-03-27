// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Contributions;
using GitFyle.Core.Api.Models.Foundations.Contributions.Exceptions;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Contributions;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/contributions")]
    public class ContributionsController : RESTFulController
    {
        private readonly IContributionService contributionService;

        public ContributionsController(IContributionService contributionService) =>
            this.contributionService = contributionService;

        [HttpPost]
        public async ValueTask<ActionResult<Contribution>> PostContributionAsync(Contribution contribution)
        {
            try
            {
                Contribution addedContribution =
                    await this.contributionService.AddContributionAsync(contribution);

                return Created(addedContribution);
            }
            catch (ContributionValidationException repositoryValidationException)
            {
                return BadRequest(repositoryValidationException.InnerException);
            }
            catch (ContributionDependencyValidationException repositoryDependencyValidationException)
                when (repositoryDependencyValidationException.InnerException is AlreadyExistsContributionException)
            {
                return Conflict(repositoryDependencyValidationException.InnerException);
            }
            catch (ContributionDependencyValidationException repositoryDependencyValidationException)
            {
                return BadRequest(repositoryDependencyValidationException.InnerException);
            }
            catch (ContributionDependencyException repositoryDependencyException)
            {
                return InternalServerError(repositoryDependencyException);
            }
            catch (ContributionServiceException repositoryServiceException)
            {
                return InternalServerError(repositoryServiceException);
            }

        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Contribution>>> GetAllContributionsAsync()
        {
            try
            {
                IQueryable<Contribution> contributions =
                     await this.contributionService.RetrieveAllContributionsAsync();

                return Ok(contributions);
            }
            catch (ContributionDependencyException contributionDependencyException)
            {
                return InternalServerError(contributionDependencyException);
            }
            catch (ContributionServiceException contributionServiceException)
            {
                return InternalServerError(contributionServiceException);
            }
        }

        [HttpGet("{contributionId}")]
        public async ValueTask<ActionResult<Contribution>> GetContributionByIdAsync(Guid contributionId)
        {
            try
            {
                Contribution contribution = 
                        await this.contributionService.RetrieveContributionByIdAsync(contributionId);

                return Ok(contribution);
            }
            catch (ContributionValidationException contributionValidationException)
                when (contributionValidationException.InnerException is NotFoundContributionException)
            {
                return NotFound(contributionValidationException.InnerException);
            }
            catch (ContributionValidationException contributionValidationException)
            {
                return BadRequest(contributionValidationException.InnerException);
            }
            catch (ContributionDependencyValidationException contributionDependencyValidationException)
            {
                return BadRequest(contributionDependencyValidationException.InnerException);
            }
            catch (ContributionDependencyException contributionDependencyException)
            {
                return InternalServerError(contributionDependencyException);
            }
            catch (ContributionServiceException contributionServiceException)
            {
                return InternalServerError(contributionServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Contribution>> PutContributionAsync(Contribution contribution)
        {
            try
            {
                Contribution modifiedContribution =
                    await this.contributionService.ModifyContributionAsync(contribution);

                return Ok(modifiedContribution);
            }
            catch (ContributionValidationException contributionValidationException)
                when (contributionValidationException.InnerException is NotFoundContributionException)
            {
                return NotFound(contributionValidationException.InnerException);
            }
            catch (ContributionValidationException contributionValidationException)
            {
                return BadRequest(contributionValidationException.InnerException);
            }
            catch (ContributionDependencyValidationException contributionDependencyValidationException)
                when (contributionDependencyValidationException.InnerException is AlreadyExistsContributionException)
            {
                return Conflict(contributionDependencyValidationException.InnerException);
            }
            catch (ContributionDependencyValidationException contributionDependencyValidationException)
            {
                return BadRequest(contributionDependencyValidationException.InnerException);
            }
            catch (ContributionDependencyException contributionDependencyException)
            {
                return InternalServerError(contributionDependencyException);
            }
            catch (ContributionServiceException contributionServiceException)
            {
                return InternalServerError(contributionServiceException);
            }
        }

        [HttpDelete("{contributionId}")]
        public async ValueTask<ActionResult<Contribution>> DeleteContributionByIdAsync(Guid contributionId)
        {
            try
            {
                Contribution deleteContribution =
                    await this.contributionService.RemoveContributionByIdAsync(contributionId);

                return Ok(deleteContribution);
            }
            catch (ContributionValidationException contributionValidationException)
                when (contributionValidationException.InnerException is NotFoundContributionException)
            {
                return NotFound(contributionValidationException.InnerException);
            }
            catch (ContributionValidationException contributionValidationException)
            {
                return BadRequest(contributionValidationException.InnerException);
            }
            catch (ContributionDependencyValidationException contributionDependencyValidationException)
                when (contributionDependencyValidationException.InnerException is LockedContributionException)
            {
                return Locked(contributionDependencyValidationException.InnerException);
            }
            catch (ContributionDependencyValidationException contributionDependencyValidationException)
            {
                return BadRequest(contributionDependencyValidationException.InnerException);
            }
            catch (ContributionDependencyException contributionDependencyException)
            {
                return InternalServerError(contributionDependencyException);
            }
            catch (ContributionServiceException contributionServiceException)
            {
                return InternalServerError(contributionServiceException);
            }
        }
    }
}
