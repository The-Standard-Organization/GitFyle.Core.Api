// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Models.Foundations.Sources.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourcesController : RESTFulController
    {
        private readonly ISourceService sourceService;

        public SourcesController(ISourceService sourceService) =>
            this.sourceService = sourceService;

        [HttpPost]
        public async ValueTask<ActionResult<Source>> PostSourceAsync(Source source)
        {
            try
            {
                Source addedSource =
                    await sourceService.AddSourceAsync(source);

                return Created(addedSource);
            }
            catch (SourceValidationException sourceValidationException)
            {
                return BadRequest(sourceValidationException.InnerException);
            }
            catch (SourceDependencyValidationException sourceDependencyValidationException)
                when (sourceDependencyValidationException.InnerException is AlreadyExistsSourceException)
            {
                return Conflict(sourceDependencyValidationException.InnerException);
            }
            catch (SourceDependencyValidationException sourceDependencyValidationException)
            {
                return BadRequest(sourceDependencyValidationException.InnerException);
            }
            catch (SourceDependencyException sourceDependencyException)
            {
                return InternalServerError(sourceDependencyException);
            }
            catch (SourceServiceException sourceServiceException)
            {
                return InternalServerError(sourceServiceException);
            }
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Source>>> GetAsync()
        {
            try
            {
                IQueryable<Source> sourcees =
                    await this.sourceService.RetrieveAllSourcesAsync();

                return Ok(sourcees);
            }
            catch (SourceDependencyException sourceDependencyException)
            {
                return InternalServerError(sourceDependencyException);
            }
            catch (SourceServiceException sourceServiceException)
            {
                return InternalServerError(sourceServiceException);
            }
        }

        [HttpGet("{sourceId}")]
        public async ValueTask<ActionResult<Source>> GetSourceByIdAsync(Guid sourceId)
        {
            try
            {
                Source source = await this.sourceService.RetrieveSourceByIdAsync(sourceId);

                return Ok(source);
            }
            catch (SourceValidationException sourceValidationException)
            {
                return BadRequest(sourceValidationException.InnerException);
            }
            catch (SourceDependencyValidationException sourceDependencyValidationException)
            {
                return BadRequest(sourceDependencyValidationException.InnerException);
            }
        }
    }
}
