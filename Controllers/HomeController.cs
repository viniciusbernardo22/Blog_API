using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        //[ApiKey]
        //HealthCheck
        public ActionResult Get([FromServices] IConfiguration config)
        {
            return Ok(new
            {
                Ambiente = config["Env"],
                Versao = config["Versao"],
                NomeApp = config["NomeApp"],
                Host = Environment.MachineName,
                DataHora = DateTime.Now,
                Contato = "vini383@gmail.com"
            });
        }

    }
}
