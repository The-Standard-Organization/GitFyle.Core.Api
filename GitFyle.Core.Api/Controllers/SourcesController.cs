// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Sources;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class SourcesController : RESTFulController
    {
        private readonly ISourceService sourceService;

        public SourcesController(ISourceService sourceService) =>
            this.sourceService = sourceService;

        [HttpPost]
        public ValueTask<ActionResult<Source>> PostSourceAsync(Source source) =>
        TryCatch((ReturningSourceFunction)(async () =>
        {
            Source addedSource =
                await sourceService.AddSourceAsync(source);

            return Created(addedSource);
        }));

        [HttpGet]
        public ValueTask<ActionResult<IQueryable<Source>>> GetAllSourcesAsync() =>
        TryCatch((ReturningSourcesFunction)(async () =>
        {
            IQueryable<Source> sourcees =
                await this.sourceService.RetrieveAllSourcesAsync();

            return Ok(sourcees);
        }));

        [HttpGet("{sourceId}")]
        public ValueTask<ActionResult<Source>> GetSourceByIdAsync(Guid sourceId) =>
        TryCatch((ReturningSourceFunction)(async () =>
        {
            Source source =
                await this.sourceService.RetrieveSourceByIdAsync(sourceId);

            return Ok(source);
        }));

        [HttpPut]
        public ValueTask<ActionResult<Source>> PutSourceAsync(Source source) =>
        TryCatch((ReturningSourceFunction)(async () =>
        {
            Source modifiedSource =
                await this.sourceService.ModifySourceAsync(source);

            return Ok(modifiedSource);
        }));

        [HttpDelete("{sourceId}")]
        public ValueTask<ActionResult<Source>> DeleteSourceByIdAsync(Guid sourceId) =>
        TryCatch((ReturningSourceFunction)(async () =>
        {
            Source deletedSource =
                await this.sourceService.RemoveSourceByIdAsync(sourceId);

            return Ok(deletedSource);
        }));
    }
}
