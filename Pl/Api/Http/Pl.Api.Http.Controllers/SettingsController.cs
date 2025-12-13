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

[Route(SettingsApiEndpointsConstants.BaseConstant)]
public sealed class SettingsController(ISettingsBl settingsBl) : BaseApiController
{
    [HttpGet]
    [Route(SettingsApiEndpointsConstants.ByIdConstant)]
    [ProducesResponseType(typeof(RestApiResponse<SettingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiResponse<SettingDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestApiResponse<SettingDto>>> Get([FromRoute] int id, [FromQuery] SettingsConvertParams? convertParams)
    {
        var dto = (await settingsBl.GetAsync(id, convertParams)).ToDto();
        return Ok(RestApiResponseBuilder<SettingDto>.Success(dto));
    }

    [HttpGet]
    [Route(IBaseApiRoutesConstants.GetByFilterSuffix)]
    [ProducesResponseType(typeof(RestApiResponse<SearchResult<SettingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<SearchResult<SettingDto>>>> Get([FromQuery] SettingsSearchParams searchParams, [FromQuery] SettingsConvertParams? convertParams)
    {
        var searchResult = (await settingsBl.GetAsync(searchParams, convertParams)).Map(SettingsMapper.ToDto);
        return Ok(RestApiResponseBuilder<SearchResult<SettingDto>>.Success(searchResult));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RestApiResponse<int>), StatusCodes.Status201Created)]
    public async Task<ActionResult<RestApiResponse<int>>> Create([FromBody] SettingDto dto)
    {
        dto.Id = await settingsBl.AddOrUpdateAsync(dto.ToEntity());
        return Created(string.Empty, RestApiResponseBuilder<int>.Success(dto.Id));
    }

    [HttpPut]
    [Route(SettingsApiEndpointsConstants.ByIdConstant)]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromRoute] int id, [FromBody] SettingDto dto)
    {
        dto.Id = id;
        await settingsBl.AddOrUpdateAsync(dto.ToEntity());
        return Ok(RestApiResponseBuilder<NoContent>.Success(new()));
    }

    [HttpDelete]
    [Route(SettingsApiEndpointsConstants.ByIdConstant)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await settingsBl.DeleteAsync(id);
        return NoContent();
    }
}
