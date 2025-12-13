using Microsoft.AspNetCore.Mvc;

using System.Net.Mime;

namespace Pl.Api.Http.Controllers.Core;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public abstract class BaseApiController : ControllerBase
{
}
