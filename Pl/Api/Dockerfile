# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Pl/Api/Api.csproj", "Pl/Api/"]
COPY ["Bl/Bl.DependencyInjection/Bl.DependencyInjection.csproj", "Bl/Bl.DependencyInjection/"]
COPY ["Bl/Bl.Implementations/Bl.Implementations.csproj", "Bl/Bl.Implementations/"]
COPY ["Common/Common.Helpers/Common.Helpers.csproj", "Common/Common.Helpers/"]
COPY ["Common/Common.Constants/Common.Constants.csproj", "Common/Common.Constants/"]
COPY ["Dal/Dal.Interfaces/Dal.Interfaces.csproj", "Dal/Dal.Interfaces/"]
COPY ["Common/Common.ConvertParams/Common.ConvertParams.csproj", "Common/Common.ConvertParams/"]
COPY ["Common/Common.SearchParams/Common.SearchParams.csproj", "Common/Common.SearchParams/"]
COPY ["Common/Common.Enums/Common.Enums.csproj", "Common/Common.Enums/"]
COPY ["Domain/Entities/Entities.csproj", "Domain/Entities/"]
COPY ["Domain/Contracts/Contracts.csproj", "Domain/Contracts/"]
COPY ["Dal/Dal.DbModels/Dal.DbModels.csproj", "Dal/Dal.DbModels/"]
COPY ["Common/Common.Attributes/Common.Attributes.csproj", "Common/Common.Attributes/"]
COPY ["Bl/Bl.Interfaces/Bl.Interfaces.csproj", "Bl/Bl.Interfaces/"]
COPY ["Dal/Dal.DependencyInjection/Dal.DependencyInjection.csproj", "Dal/Dal.DependencyInjection/"]
COPY ["Dal/Dal.Implementations/Dal.Implementations.csproj", "Dal/Dal.Implementations/"]
COPY ["Common/Common.Exceptions/Common.Exceptions.csproj", "Common/Common.Exceptions/"]
COPY ["Dal/Dal.Ef/Dal.Ef.csproj", "Dal/Dal.Ef/"]
RUN dotnet restore "./Pl/Api/Api.csproj"
COPY . .
WORKDIR "/src/Pl/Api"
RUN dotnet build "./Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]