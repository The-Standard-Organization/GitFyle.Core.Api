// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Repositories;
using GitFyle.Core.Api.Models.Foundations.Repositories.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Repositories;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepositoriesController : RESTFulController
    {
        private readonly IRepositoryService repositoryService;

        public RepositoriesController(IRepositoryService repositoryService) =>
            this.repositoryService = repositoryService;

        [HttpPost]
        public async ValueTask<ActionResult<Repository>> PostRepositoryAsync(Repository repository)
        {
            try 
            { 
                Repository addedRepository =
                    await this.repositoryService.AddRepositoryAsync(repository);

                return Created(addedRepository);
             }
            catch (RepositoryValidationException configurationValidationException)
            {
                return BadRequest(configurationValidationException.InnerException);
            }
            catch (RepositoryDependencyValidationException configurationDependencyValidationException)
            {
                return BadRequest(configurationDependencyValidationException.InnerException);
            }
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Repository>>> GetRepositoriesAsync()
        {
            try
            {
                IQueryable<Repository> repositories =
                    await this.repositoryService.RetrieveAllRepositoriesAsync();

                return Ok(repositories);
            }
            catch (RepositoryDependencyException repositoryDependencyException)
            {
                return InternalServerError(repositoryDependencyException);
            }
            catch (RepositoryServiceException repositoryServiceException)
            {
                return InternalServerError(repositoryServiceException);
            }
        }

        [HttpGet("{repositoryId}")]
        public async ValueTask<ActionResult<Repository>> GetRepositoryByIdAsync(Guid repositoryId)
        {
            try
            {
                Repository repository =
                    await this.repositoryService.RetrieveRepositoryByIdAsync(repositoryId);

                return Ok(repository);
            }
            catch (RepositoryValidationException repositoryValidationException)
                when (repositoryValidationException.InnerException is NotFoundRepositoryException)
            {
                return NotFound(repositoryValidationException.InnerException);
            }
            catch (RepositoryValidationException repositoryValidationException)
            {
                return BadRequest(repositoryValidationException.InnerException);
            }
            catch (RepositoryDependencyValidationException repositoryDependencyValidationException)
            {
                return BadRequest(repositoryDependencyValidationException.InnerException);
            }
            catch (RepositoryDependencyException repositoryDependencyException)
            {
                return InternalServerError(repositoryDependencyException);
            }
            catch (RepositoryServiceException repositoryServiceException)
            {
                return InternalServerError(repositoryServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Repository>> PutRepositoryAsync(Repository repository)
        {
            try
            {
                Repository modifiedRepository =
                    await this.repositoryService.ModifyRepositoryAsync(repository);

                return Ok(modifiedRepository);
            }
            catch (RepositoryValidationException repositoryValidationException)
                when (repositoryValidationException.InnerException is NotFoundRepositoryException)
            {
                return NotFound(repositoryValidationException.InnerException);
            }
            catch (RepositoryValidationException repositoryValidationException)
            {
                return BadRequest(repositoryValidationException.InnerException);
            }
            catch (RepositoryDependencyValidationException repositoryDependencyValidationException)
                when (repositoryDependencyValidationException.InnerException is AlreadyExistsRepositoryException)
            {
                return Conflict(repositoryDependencyValidationException.InnerException);
            }
            catch (RepositoryDependencyValidationException repositoryDependencyValidationException)
            {
                return BadRequest(repositoryDependencyValidationException.InnerException);
            }
            catch (RepositoryDependencyException repositoryDependencyException)
            {
                return InternalServerError(repositoryDependencyException);
            }
            catch (RepositoryServiceException repositoryServiceException)
            {
                return InternalServerError(repositoryServiceException);
            }
        }

        [HttpDelete("{repositoryId}")]
        public async ValueTask<ActionResult<Repository>> DeleteRepositoryByIdAsync(Guid repositoryId)
        {
            try
            {
                Repository deleteRepository =
                    await this.repositoryService.RemoveRepositoryByIdAsync(repositoryId);

                return Ok(deleteRepository);
            }
            catch (RepositoryValidationException repositoryValidationException)
                when (repositoryValidationException.InnerException is NotFoundRepositoryException)
            {
                return NotFound(repositoryValidationException.InnerException);
            }
            catch (RepositoryValidationException repositoryValidationException)
            {
                return BadRequest(repositoryValidationException.InnerException);
            }
            catch (RepositoryDependencyValidationException repositoryDependencyValidationException)
                when (repositoryDependencyValidationException.InnerException is LockedRepositoryException)
            {
                return Locked(repositoryDependencyValidationException.InnerException);
            }
            catch (RepositoryDependencyValidationException repositoryDependencyValidationException)
            {
                return BadRequest(repositoryDependencyValidationException.InnerException);
            }
            catch (RepositoryDependencyException repositoryDependencyException)
            {
                return InternalServerError(repositoryDependencyException);
            }
            catch (RepositoryServiceException repositoryServiceException)
            {
                return InternalServerError(repositoryServiceException);
            }
        }
    }
}
