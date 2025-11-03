using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Core;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public abstract class BaseApiController : ControllerBase
{
}
