// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes;
using GitFyle.Core.Api.Models.Foundations.ContributionTypes.Exceptions;
using GitFyle.Core.Api.Services.Foundations.ContributionTypes;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContributionTypesController : RESTFulController
    {
        private readonly IContributionTypeService contributionTypeService;

        public ContributionTypesController(IContributionTypeService contributionTypeService) =>
            this.contributionTypeService = contributionTypeService;

        [HttpPost]
        public async ValueTask<ActionResult<ContributionType>> PostContributionTypeAsync(
            ContributionType contributionType)
        {
            try
            {
                ContributionType addedContributionType =
                    await this.contributionTypeService.AddContributionTypeAsync(contributionType);

                return Created(addedContributionType);
            }
            catch (ContributionTypeValidationException contributionTypeValidationException)
            {
                return BadRequest(contributionTypeValidationException.InnerException);
            }
            catch (ContributionTypeDependencyValidationException contributionTypeDependencyValidationException)
                when (contributionTypeDependencyValidationException.InnerException is 
                    AlreadyExistsContributionTypeException)
            {
                return Conflict(contributionTypeDependencyValidationException.InnerException);
            }
            catch (ContributionTypeDependencyValidationException contributionTypeDependencyValidationException)
            {
                return BadRequest(contributionTypeDependencyValidationException.InnerException);
            }
            catch (ContributionTypeDependencyException contributionTypeDependencyException)
            {
                return InternalServerError(contributionTypeDependencyException);
            }
            catch (ContributionTypeServiceException contributionTypeServiceException)
            {
                return InternalServerError(contributionTypeServiceException);
            }
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<ContributionType>>> GetContributionTypesAsync()
        {
            try
            {
                IQueryable<ContributionType> repositories =
                    await this.contributionTypeService.RetrieveAllContributionTypesAsync();

                return Ok(repositories);
            }
            catch (ContributionTypeDependencyException contributionTypeDependencyException)
            {
                return InternalServerError(contributionTypeDependencyException);
            }
            catch (ContributionTypeServiceException contributionTypeServiceException)
            {
                return InternalServerError(contributionTypeServiceException);
            }
        }

        [HttpGet("{contributionTypeId}")]
        public async ValueTask<ActionResult<ContributionType>> GetContributionTypeByIdAsync(Guid contributionTypeId)
        {
            try
            {
                ContributionType contributionType =
                    await this.contributionTypeService.RetrieveContributionTypeByIdAsync(contributionTypeId);

                return Ok(contributionType);
            }
            catch (ContributionTypeValidationException contributionTypeValidationException)
                when (contributionTypeValidationException.InnerException is NotFoundContributionTypeException)
            {
                return NotFound(contributionTypeValidationException.InnerException);
            }
            catch (ContributionTypeValidationException contributionTypeValidationException)
            {
                return BadRequest(contributionTypeValidationException.InnerException);
            }
            catch (ContributionTypeDependencyValidationException contributionTypeDependencyValidationException)
            {
                return BadRequest(contributionTypeDependencyValidationException.InnerException);
            }
            catch (ContributionTypeDependencyException contributionTypeDependencyException)
            {
                return InternalServerError(contributionTypeDependencyException);
            }
            catch (ContributionTypeServiceException contributionTypeServiceException)
            {
                return InternalServerError(contributionTypeServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<ContributionType>> PutContributionTypeAsync(
            ContributionType contributionType)
        {
            try
            {
                ContributionType modifiedContributionType =
                    await this.contributionTypeService.ModifyContributionTypeAsync(contributionType);

                return Ok(modifiedContributionType);
            }
            catch (ContributionTypeValidationException contributionTypeValidationException)
                when (contributionTypeValidationException.InnerException is NotFoundContributionTypeException)
            {
                return NotFound(contributionTypeValidationException.InnerException);
            }
            catch (ContributionTypeValidationException contributionTypeValidationException)
            {
                return BadRequest(contributionTypeValidationException.InnerException);
            }
            catch (ContributionTypeDependencyValidationException contributionTypeDependencyValidationException)
                when (contributionTypeDependencyValidationException.InnerException is 
                    AlreadyExistsContributionTypeException)
            {
                return Conflict(contributionTypeDependencyValidationException.InnerException);
            }
            catch (ContributionTypeDependencyValidationException contributionTypeDependencyValidationException)
              when (contributionTypeDependencyValidationException.InnerException is 
                InvalidReferenceContributionTypeException)
            {
                return FailedDependency(contributionTypeDependencyValidationException.InnerException);
            }
            catch (ContributionTypeDependencyValidationException contributionTypeDependencyValidationException)
            {
                return BadRequest(contributionTypeDependencyValidationException.InnerException);
            }
            catch (ContributionTypeDependencyException contributionTypeDependencyException)
            {
                return InternalServerError(contributionTypeDependencyException);
            }
            catch (ContributionTypeServiceException contributionTypeServiceException)
            {
                return InternalServerError(contributionTypeServiceException);
            }
        }
    }
}
