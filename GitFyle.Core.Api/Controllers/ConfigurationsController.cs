// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using GitFyle.Core.Api.Models.Foundations.Configurations;
using GitFyle.Core.Api.Models.Foundations.Configurations.Exceptions;
using GitFyle.Core.Api.Services.Foundations.Configurations;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationsController : RESTFulController
    {
        private readonly IConfigurationService configurationService;

        public ConfigurationsController(IConfigurationService configurationService) =>
            this.configurationService = configurationService;

        [HttpPost]
        public async ValueTask<ActionResult<Configuration>> PostConfigurationAsync(Configuration configuration)
        {
            try
            {
                Configuration addedConfiguration =
                    await this.configurationService.AddConfigurationAsync(configuration);

                return Created(addedConfiguration);
            }
            catch (ConfigurationValidationException configurationValidationException)
            {
                return BadRequest(configurationValidationException.InnerException);
            }
            catch (ConfigurationDependencyValidationException configurationDependencyValidationException)
                when (configurationDependencyValidationException.InnerException is AlreadyExistsConfigurationException)
            {
                return Conflict(configurationDependencyValidationException.InnerException);
            }
            catch (ConfigurationDependencyValidationException configurationDependencyValidationException)
            {
                return BadRequest(configurationDependencyValidationException.InnerException);
            }
            catch (ConfigurationDependencyException configurationDependencyException)
            {
                return InternalServerError(configurationDependencyException);
            }
            catch (ConfigurationServiceException configurationServiceException)
            {
                return InternalServerError(configurationServiceException);
            }
        }

        [HttpGet("{configurationId}")]
        public async ValueTask<ActionResult<Configuration>> GetConfigurationByIdAsync(Guid configurationId)
        {
            try
            {
                Configuration configuration =
                    await this.configurationService.RetrieveConfigurationByIdAsync(configurationId);

                return Ok(configuration);
            }
            catch (ConfigurationValidationException configurationValidationException)
                when (configurationValidationException.InnerException is NotFoundConfigurationException)
            {
                return NotFound(configurationValidationException.InnerException);
            }
            catch (ConfigurationValidationException configurationValidationException)
            {
                return BadRequest(configurationValidationException.InnerException);
            }
            catch (ConfigurationDependencyValidationException configurationDependencyValidationException)
            {
                return BadRequest(configurationDependencyValidationException.InnerException);
            }
            catch (ConfigurationDependencyException configurationDependencyException)
            {
                return InternalServerError(configurationDependencyException);
            }
            catch (ConfigurationServiceException configurationServiceException)
            {
                return InternalServerError(configurationServiceException);
            }
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Configuration>>> GetConfigurationsAsync()
        {
            try
            {
                IQueryable<Configuration> configurations =
                    await this.configurationService.RetrieveAllConfigurationsAsync();

                return Ok(configurations);
            }
            catch (ConfigurationDependencyException configurationDependencyException)
            {
                return InternalServerError(configurationDependencyException);
            }
            catch (ConfigurationServiceException configurationServiceException)
            {
                return InternalServerError(configurationServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Configuration>> PutConfigurationAsync(Configuration configuration)
        {
            try
            {
                Configuration modifiedConfiguration =
                    await this.configurationService.ModifyConfigurationAsync(configuration);

                return Ok(modifiedConfiguration);
            }
            catch (ConfigurationValidationException configurationValidationException)
                when (configurationValidationException.InnerException is NotFoundConfigurationException)
            {
                return NotFound(configurationValidationException.InnerException);
            }
            catch (ConfigurationValidationException configurationValidationException)
            {
                return BadRequest(configurationValidationException.InnerException);
            }
            catch (ConfigurationDependencyValidationException configurationDependencyValidationException)
                when (configurationDependencyValidationException.InnerException is AlreadyExistsConfigurationException)
            {
                return Conflict(configurationDependencyValidationException.InnerException);
            }
            catch (ConfigurationDependencyValidationException configurationDependencyValidationException)
            {
                return BadRequest(configurationDependencyValidationException.InnerException);
            }
            catch (ConfigurationDependencyException configurationDependencyException)
            {
                return InternalServerError(configurationDependencyException);
            }
            catch (ConfigurationServiceException configurationServiceException)
            {
                return InternalServerError(configurationServiceException);
            }
        }

        [HttpDelete("{configurationId}")]
        public async ValueTask<ActionResult<Configuration>> DeleteConfigurationByIdAsync(Guid configurationId)
        {
            Configuration deleteConfiguration = 
                await this.configurationService.RemoveConfigurationByIdAsync(configurationId);

            return Ok(deleteConfiguration);
        }
    }
}
