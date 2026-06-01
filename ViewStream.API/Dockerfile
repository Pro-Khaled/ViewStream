# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7163
EXPOSE 5062


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ViewStream.API/ViewStream.API.csproj", "ViewStream.API/"]
COPY ["ViewStream.Application/ViewStream.Application.csproj", "ViewStream.Application/"]
COPY ["ViewStream.Domain/ViewStream.Domain.csproj", "ViewStream.Domain/"]
COPY ["ViewStream.Shared/ViewStream.Shared.csproj", "ViewStream.Shared/"]
COPY ["ViewStream.Infrastructure/ViewStream.Infrastructure.csproj", "ViewStream.Infrastructure/"]
RUN dotnet restore "./ViewStream.API/ViewStream.API.csproj"
COPY . .
WORKDIR "/src/ViewStream.API"
RUN dotnet build "./ViewStream.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ViewStream.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ViewStream.API.dll"]