# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Pl/Api/Pl.Api.Host/Pl.Api.Host.csproj", "Pl/Api/Pl.Api.Host/"]
COPY ["Bl/Bl.DependencyInjection/Bl.DependencyInjection.csproj", "Bl/Bl.DependencyInjection/"]
COPY ["Bl/Bl.Implementations/Bl.Implementations.csproj", "Bl/Bl.Implementations/"]
COPY ["Common/Common.Helpers/Common.Helpers.csproj", "Common/Common.Helpers/"]
COPY ["Common/Common.Constants/Common.Constants.csproj", "Common/Common.Constants/"]
COPY ["Dal/Dal.Interfaces/Dal.Interfaces.csproj", "Dal/Dal.Interfaces/"]
COPY ["Common/Common.ConvertParams/Common.ConvertParams.csproj", "Common/Common.ConvertParams/"]
COPY ["Common/Common.SearchParams/Common.SearchParams.csproj", "Common/Common.SearchParams/"]
COPY ["Common/Common.Enums/Common.Enums.csproj", "Common/Common.Enums/"]
COPY ["Domain/Entities/Entities.csproj", "Domain/Entities/"]
COPY ["Dal/Dal.DbModels/Dal.DbModels.csproj", "Dal/Dal.DbModels/"]
COPY ["Common/Common.Attributes/Common.Attributes.csproj", "Common/Common.Attributes/"]
COPY ["Bl/Bl.Interfaces/Bl.Interfaces.csproj", "Bl/Bl.Interfaces/"]
COPY ["Dal/Dal.DependencyInjection/Dal.DependencyInjection.csproj", "Dal/Dal.DependencyInjection/"]
COPY ["Dal/Dal.Implementations/Dal.Implementations.csproj", "Dal/Dal.Implementations/"]
COPY ["Common/Common.Exceptions/Common.Exceptions.csproj", "Common/Common.Exceptions/"]
COPY ["Dal/Dal.Ef/Dal.Ef.csproj", "Dal/Dal.Ef/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Composition/Pl.Api.Http.Composition.csproj", "Pl/Api/Http/Pl.Api.Http.Composition/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Controllers/Pl.Api.Http.Controllers.csproj", "Pl/Api/Http/Pl.Api.Http.Controllers/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Dtos/Pl.Api.Http.Dtos.csproj", "Pl/Api/Http/Pl.Api.Http.Dtos/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Mappers/Pl.Api.Http.Mappers.csproj", "Pl/Api/Http/Pl.Api.Http.Mappers/"]
COPY ["Pl/Api/Http/Pl.Api.Http.Middlewares/Pl.Api.Http.Middlewares.csproj", "Pl/Api/Http/Pl.Api.Http.Middlewares/"]
RUN dotnet restore "./Pl/Api/Pl.Api.Host/Pl.Api.Host.csproj"
COPY . .
WORKDIR "/src/Pl/Api/Pl.Api.Host"
RUN dotnet build "./Pl.Api.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pl.Api.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pl.Api.Host.dll"]