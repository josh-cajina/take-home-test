using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Loan.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HealthcheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() 
    {
        return Ok();
    }
}
