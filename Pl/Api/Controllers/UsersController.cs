using System.Threading.Tasks;

using Api.Controllers.Core;
using Api.Extensions;
using Api.Infrastructure.Core;
using Api.Infrastructure.Models;
using Api.Infrastructure.Responses.Core;

using Bl.Interfaces;

using Common.Constants.ApiEndpoints;
using Common.Constants.ApiEndpoints.Core;
using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using Gen.IdentityService.ApplicationUserService;
using Gen.IdentityService.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Common.Constants.IdentityServiceConstants;

namespace Api.Controllers;

[Route(UsersApiEndpointsConstants.BaseConstant)]
public sealed class UsersController : BaseApiController
{
    private readonly ApplicationUserService.ApplicationUserServiceClient _applicationUserServiceClient;
    private readonly IUsersBl _usersBl;

    public UsersController(
        ApplicationUserService.ApplicationUserServiceClient applicationUserServiceClient,
        IUsersBl usersBl)
    {
        _applicationUserServiceClient = applicationUserServiceClient;
        _usersBl = usersBl;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(RestApiResponse<UserModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestApiResponse<UserModel>>> Get([FromQuery] UsersConvertParams? convertParams)
    {
        var response = UserModel.FromEntity(await _usersBl.GetAsync(User.GetUserId(), convertParams));
        return Ok(RestApiResponseBuilder<UserModel>.Success(response));
    }

    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(RestApiResponse<UserModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestApiResponse<UserModel>>> Get([FromRoute] int id, [FromQuery] UsersConvertParams? convertParams)
    {
        var response = UserModel.FromEntity(await _usersBl.GetAsync(id, convertParams));
        return Ok(RestApiResponseBuilder<UserModel>.Success(response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(RestApiResponse<SearchResult<UserModel>>), StatusCodes.Status200OK)]
    [Route(IBaseApiRoutesConstants.GetByFilterSuffix)]
    public async Task<ActionResult<RestApiResponse<SearchResult<UserModel>>>> Get([FromQuery] UsersSearchParams searchParams, [FromQuery] UsersConvertParams? convertParams)
    {
        var response = (await _usersBl.GetAsync(searchParams, convertParams)).Map(UserModel.FromEntitiesList);
        return Ok(RestApiResponseBuilder<SearchResult<UserModel>>.Success(response));
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(ApplicationUserRole.Developer)},{nameof(ApplicationUserRole.Admin)}")]
    [ProducesResponseType(typeof(RestApiResponse<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RestApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RestApiResponse<int>>> Create([FromBody] UserModel request)
    {
        request.Id = 0;
        var user = UserModel.ToEntity(request);

        var grpcResponse = await _applicationUserServiceClient.CreateAsync(user.ApplicationUser);
        user.ApplicationUserId = grpcResponse.Id;

        request.Id = await _usersBl.AddOrUpdateAsync(user);

        await _applicationUserServiceClient.AddClaimAsync(
            new AddClaimRequest
            {
                ApplicationUserId = user.ApplicationUserId,
                Type = CustomJwtClaimTypes.UserId,
                Value = request.Id.ToString()
            });

        return Created(string.Empty, RestApiResponseBuilder<int>.Success(request.Id));
    }

    [HttpPut]
    [Authorize]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromBody] UserModel request)
    {
        var user = UserModel.ToEntity(request);
        user.Id = User.GetUserId();
        user.ApplicationUserId = User.GetApplicationUserId();
        user.ApplicationUser!.Id = User.GetApplicationUserId();

        await _applicationUserServiceClient.UpdateAsync(user.ApplicationUser);
        await _usersBl.AddOrUpdateAsync(user);

        return Ok(RestApiResponseBuilder<NoContent>.Success(new NoContent()));
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = $"{nameof(ApplicationUserRole.Developer)},{nameof(ApplicationUserRole.Admin)}")]
    [ProducesResponseType(typeof(RestApiResponse<NoContent>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RestApiResponse<NoContent>>> Update([FromRoute] int id, [FromBody] UserModel request)
    {
        var dbUser = await _usersBl.GetAsync(id);
        var user = UserModel.ToEntity(request);
        if (user.ApplicationUser is not null)
        {
            user.ApplicationUser.Id = dbUser.ApplicationUserId;
        }
        user.ApplicationUserId = dbUser.ApplicationUserId;
        user.Id = id;

        await _applicationUserServiceClient.UpdateAsync(user.ApplicationUser);
        await _usersBl.AddOrUpdateAsync(user);

        return Ok(RestApiResponseBuilder<NoContent>.Success(new NoContent()));
    }

    [HttpDelete]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete()
    {
        await _applicationUserServiceClient.DeleteAsync(new DeleteApplicationUserRequest { Id = User.GetApplicationUserId() });
        await _usersBl.DeleteAsync(User.GetUserId());
        return NoContent();
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = $"{nameof(ApplicationUserRole.Developer)},{nameof(ApplicationUserRole.Admin)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var dbUser = await _usersBl.GetAsync(id);
        await _applicationUserServiceClient.DeleteAsync(new DeleteApplicationUserRequest { Id = dbUser.ApplicationUserId });
        await _usersBl.DeleteAsync(dbUser.Id);
        return NoContent();
    }
}
