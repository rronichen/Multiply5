using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MultiplyApi.Models.Requests;
using MultiplyApi.Models.Responses;
using MultiplyApi.Services;

namespace MultiplyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("PerClientFixedWindow")]
public class MultiplyController : ControllerBase
{
    private readonly IMultiplyService _multiplyService;

    public MultiplyController(IMultiplyService multiplyService)
    {
        _multiplyService = multiplyService;
    }

    [HttpPost("check")]
    public ActionResult<CheckMultiplyResponse> Check([FromBody] CheckMultiplyRequest request)
    {
        if (!_multiplyService.TryParseValue(request.Value, out var parsedValue))
            return BadRequest("Value must be a valid integer between 0 and 10000.");

        return Ok(new CheckMultiplyResponse(_multiplyService.IsMultiplyOfFive(parsedValue)));
    }
}