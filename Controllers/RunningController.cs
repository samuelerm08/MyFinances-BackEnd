using Microsoft.AspNetCore.Mvc;

namespace MyFinances.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunningController : ControllerBase
    {
        public ActionResult Get()
        {
            return Ok("Service is working correctly.");
        }
    }
}
