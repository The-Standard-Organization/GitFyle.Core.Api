// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using RESTFulSense.Clients;

namespace GitFyle.Core.Api.Tests.Acceptance.Brokers
{
    public partial class GitFyleCoreApiBroker
    {
        private readonly WebApplicationFactory<Program> webApplicationFactory;
        private readonly HttpClient httpClient;
        private readonly IRESTFulApiFactoryClient apiFactoryClient;

        public GitFyleCoreApiBroker()
        {
            this.webApplicationFactory = 
                new WebApplicationFactory<Program>();

            this.httpClient = 
                this.webApplicationFactory.CreateClient();
            
            this.apiFactoryClient =
                new RESTFulApiFactoryClient(
                    this.httpClient);
        }
    }
}
