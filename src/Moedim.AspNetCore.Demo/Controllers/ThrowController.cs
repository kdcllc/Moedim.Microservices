using System.ComponentModel.DataAnnotations;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Mvc;

namespace Moedim.AspNetCore.Demo.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiConventionType(typeof(DefaultApiConventions))]
[Route("v{version:apiVersion}/[controller]")]
public class ThrowController : ControllerBase
{
    // https://www.alexdresko.com/2019/08/30/problem-details-error-handling-net-core-3/
    [HttpGet("BrokeApp")]
    public ActionResult A([FromQuery] Data data)
    {
        throw new Exception("broke the app...");
    }

    [HttpGet("DivByZero")]
    public ActionResult C([FromQuery] Data data)
    {
        // force a div by zero exception
        var x = 0;
        x = 0 / x;
        return Ok();
    }

    [HttpGet("ValidationProblem1")]
    public ActionResult B([FromQuery] Data data)
    {
        ModelState.AddModelError(string.Empty, "for reals");
        return ValidationProblem();
    }

    [HttpGet("ValidationProblem2")]
    public ActionResult B2([FromQuery] Data data)
    {
        ModelState.AddModelError(string.Empty, "for reals");
        var validation = new ValidationProblemDetails(ModelState);
        throw new ProblemDetailsException(validation);
    }

    [HttpGet("BadRequestProblem")]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public ActionResult<bool> E([FromQuery] Data data)
    {
        ModelState.AddModelError(string.Empty, "for reals");
        return BadRequest(ModelState);
    }

    [HttpGet("D")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public ActionResult<bool> D([FromQuery] Data data)
    {
        return BadRequest();
    }

    public class Deets : ProblemDetails
    {
        public string Name { get; set; }
    }

    [HttpGet("D2")]
    [ProducesResponseType(typeof(Deets), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public ActionResult<bool> D2([FromQuery] Data data)
    {
        return BadRequest(new Deets { Name = "Fred" });
    }

    public class Data
    {
        [Required]
        public string Name { get; set; }
    }
}
