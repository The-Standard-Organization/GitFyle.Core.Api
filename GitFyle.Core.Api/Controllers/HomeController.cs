// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Brokers.Storages;
using Microsoft.AspNetCore.Mvc;

namespace GitFyle.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        public HomeController(IStorageBroker storageBroker)
        {
            StorageBroker = storageBroker;
        }

        public IStorageBroker StorageBroker { get; }

        [HttpGet]
        public ActionResult<string> Get() =>
            Ok("Hello, Mario. The princess is in another castle.");
    }
}
