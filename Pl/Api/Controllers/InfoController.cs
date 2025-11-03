using System;

using Api.Controllers.Core;
using Api.Infrastructure.Core;
using Api.Infrastructure.Responses.Core;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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
