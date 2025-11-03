using Gen.IdentityService.ApplicationUserService;
using Gen.IdentityService.Entities;

using Grpc.Core;

using IntegrationTests.DataFactories.Generation;
using IntegrationTests.Mocks.Grpc.Core;

using NSubstitute;

namespace IntegrationTests.Mocks.Grpc;

internal static class ApplicationUserServiceClientMock
{
    public static ApplicationUserService.ApplicationUserServiceClient CreateDefault()
    {
        var invoker = Substitute.For<CallInvoker>();
        var client = Substitute.For<ApplicationUserService.ApplicationUserServiceClient>(invoker);

        var testDataFacade = new TestDataFacade();

        client.GetAsync(Arg.Any<GetApplicationUserRequest>())
            .Returns(callInfo =>
            {
                return GrpcTestCalls.UnaryResult(testDataFacade.Create<ApplicationUser>());
            });

        client.GetByFilterAsync(Arg.Any<GetApplicationUsersRequest>())
            .Returns(callInfo =>
            {
                var response = new GetApplicationUsersResponse();
                response.ApplicationUsers.Add(testDataFacade.Create<ApplicationUser>());
                response.ApplicationUsers.Add(testDataFacade.Create<ApplicationUser>());
                response.ApplicationUsers.Add(testDataFacade.Create<ApplicationUser>());
                return GrpcTestCalls.UnaryResult(response);
            });

        client.CreateAsync(Arg.Any<ApplicationUser>())
            .Returns(callInfo =>
            {
                return GrpcTestCalls.UnaryResult(new CreateApplicationUserResponse { Id = 1 });
            });

        client.AddClaimAsync(Arg.Any<AddClaimRequest>())
            .Returns(_ => GrpcTestCalls.UnaryEmpty());

        client.UpdateAsync(Arg.Any<ApplicationUser>())
            .Returns(_ => GrpcTestCalls.UnaryEmpty());

        client.DeleteAsync(Arg.Any<DeleteApplicationUserRequest>())
            .Returns(_ => GrpcTestCalls.UnaryEmpty());

        return client;
    }
}
