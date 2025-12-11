using System.Threading.Tasks;

using Api.Controllers.Core;
using Api.Infrastructure.Core;
using Api.Infrastructure.Models;
using Api.Infrastructure.Responses.Core;

using Bl.Interfaces;

using Common.Constants.ApiEndpoints;
using Common.Constants.ApiEndpoints.Core;
using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route(SettingsApiEndpointsConstants.BaseConstant)]
public sealed class SettingsController : BaseApiController
{
    private readonly ISettingsBl _settingsBl;

    public SettingsController(ISettingsBl settingsBl)
    {
        _settingsBl = settingsBl;
    }

    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(RestApiResponse<SettingsModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestApiResponse<SettingsModel>>> Get([FromRoute] int id, [FromQuery] SettingsConvertParams? convertParams)
    {
        var response = SettingsModel.FromEntity(await _settingsBl.GetAsync(id, convertParams));
        return Ok(RestApiResponseBuilder<SettingsModel>.Success(response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(RestApiResponse<SearchResult<SettingsModel>>), StatusCodes.Status200OK)]
    [Route(IBaseApiRoutesConstants.GetByFilterSuffix)]
    public async Task<ActionResult<RestApiResponse<SearchResult<SettingsModel>>>> Get([FromQuery] SettingsSearchParams searchParams, [FromQuery] SettingsConvertParams? convertParams)
    {
        var response = (await _settingsBl.GetAsync(searchParams, convertParams)).Map(SettingsModel.FromEntitiesList);
        return Ok(RestApiResponseBuilder<SearchResult<SettingsModel>>.Success(response));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RestApiResponse<int>), StatusCodes.Status201Created)]
    public async Task<ActionResult<RestApiResponse<int>>> Create([FromBody] SettingsModel request)
    {
        request.Id = await _settingsBl.AddOrUpdateAsync(SettingsModel.ToEntity(request));
        return Created(string.Empty, RestApiResponseBuilder<int>.Success(request.Id));
    }

    [HttpPut]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromRoute] int id, [FromBody] SettingsModel request)
    {
        request.Id = id;
        await _settingsBl.AddOrUpdateAsync(SettingsModel.ToEntity(request));
        return Ok(RestApiResponseBuilder<NoContent>.Success(new NoContent()));
    }

    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _settingsBl.DeleteAsync(id);
        return NoContent();
    }
}
