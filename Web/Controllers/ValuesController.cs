using System.Collections.Generic;
using System.Web.Mvc;
using Lib;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IServiceA _serviceA;

        public ValuesController(IServiceA serviceA)
        {
            _serviceA = serviceA;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _serviceA.Fu();
            return new[] { "value1", "value2" };
        }
    }
}
