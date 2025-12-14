using Bl.Interfaces;

using Common.Constants.ApiEndpoints;
using Common.Constants.ApiEndpoints.Core;
using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Pl.Api.Http.Controllers.Core;
using Pl.Api.Http.Dtos.Core;
using Pl.Api.Http.Dtos.Models;
using Pl.Api.Http.Mappers;

namespace Pl.Api.Http.Controllers;

[Route(UsersApiEndpointsConstants.BaseConstant)]
public sealed class UsersController(IUsersBl usersBl) : BaseApiController
{
    [HttpGet]
    [Route(UsersApiEndpointsConstants.IdConstant)]
    [ProducesResponseType(typeof(RestApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestApiResponse<UserDto>>> Get([FromRoute] int id, [FromQuery] UsersConvertParams? convertParams)
    {
        var dto = (await usersBl.GetAsync(id, convertParams)).ToDto();
        return Ok(RestApiResponseBuilder<UserDto>.Success(dto));
    }

    [HttpGet]
    [Route(IBaseApiRoutesConstants.GetByFilterSuffix)]
    [ProducesResponseType(typeof(RestApiResponse<SearchResult<UserDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<SearchResult<UserDto>>>> Get([FromQuery] UsersSearchParams searchParams, [FromQuery] UsersConvertParams? convertParams)
    {
        var searchResult = (await usersBl.GetAsync(searchParams, convertParams)).Map(UsersMapper.ToDto);
        return Ok(RestApiResponseBuilder<SearchResult<UserDto>>.Success(searchResult));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RestApiResponse<int>), StatusCodes.Status201Created)]
    public async Task<ActionResult<RestApiResponse<int>>> Create([FromBody] UserDto dto)
    {
        dto.Id = await usersBl.AddOrUpdateAsync(dto.ToEntity());
        return Created(string.Empty, RestApiResponseBuilder<int>.Success(dto.Id));
    }

    [HttpPut]
    [Route(UsersApiEndpointsConstants.IdConstant)]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromRoute] int id, [FromBody] UserDto dto)
    {
        dto.Id = id;
        await usersBl.AddOrUpdateAsync(dto.ToEntity());
        return Ok(RestApiResponseBuilder<NoContent>.Success(new()));
    }

    [HttpDelete]
    [Route(UsersApiEndpointsConstants.IdConstant)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await usersBl.DeleteAsync(id);
        return NoContent();
    }
}
