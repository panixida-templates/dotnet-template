using Microsoft.AspNetCore.Mvc;

using Pl.Api.Http.Controllers.Core;
using Pl.Api.Http.Dtos.Core;

namespace Pl.Api.Http.Controllers;

[Route("api/info")]
public sealed class InfoController : BaseApiController
{
    [HttpGet]
    [Route("version")]
    [ProducesResponseType(typeof(RestApiResponse<string>), 200)]
    public IActionResult GetVersion()
    {
        return Ok(RestApiResponseBuilder<string>.Success(Environment.GetEnvironmentVariable("API_VERSION") ?? "1.0"));
    }
}
