# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Dal/Dal.Migrator/Dal.Migrator.csproj", "Dal/Dal.Migrator/"]
COPY ["Dal/Dal.DependencyInjection/Dal.DependencyInjection.csproj", "Dal/Dal.DependencyInjection/"]
COPY ["Dal/Dal.Implementations/Dal.Implementations.csproj", "Dal/Dal.Implementations/"]
COPY ["Common/Common.Exceptions/Common.Exceptions.csproj", "Common/Common.Exceptions/"]
COPY ["Dal/Dal.Ef/Dal.Ef.csproj", "Dal/Dal.Ef/"]
COPY ["Common/Common.Constants/Common.Constants.csproj", "Common/Common.Constants/"]
COPY ["Dal/Dal.DbModels/Dal.DbModels.csproj", "Dal/Dal.DbModels/"]
COPY ["Common/Common.Attributes/Common.Attributes.csproj", "Common/Common.Attributes/"]
COPY ["Common/Common.Enums/Common.Enums.csproj", "Common/Common.Enums/"]
COPY ["Dal/Dal.Interfaces/Dal.Interfaces.csproj", "Dal/Dal.Interfaces/"]
COPY ["Common/Common.ConvertParams/Common.ConvertParams.csproj", "Common/Common.ConvertParams/"]
COPY ["Common/Common.SearchParams/Common.SearchParams.csproj", "Common/Common.SearchParams/"]
COPY ["Domain/Entities/Entities.csproj", "Domain/Entities/"]
COPY ["Domain/Contracts/Contracts.csproj", "Domain/Contracts/"]
RUN dotnet restore "./Dal/Dal.Migrator/Dal.Migrator.csproj"
COPY . .
WORKDIR "/src/Dal/Dal.Migrator"
RUN dotnet build "./Dal.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dal.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dal.Migrator.dll"]
