using Microsoft.AspNetCore.Mvc;

namespace SistemaFinanciero.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunningController : ControllerBase
    {
        public ActionResult Get()
        {
            return Ok("Servicio trabajando correctamente.");
        }
    }
}
