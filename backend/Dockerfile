FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BallastLane.ProductManagement.API/BallastLane.ProductManagement.API.csproj", "src/BallastLane.ProductManagement.API/"]
COPY ["src/BallastLane.ProductManagement.Domain/BallastLane.ProductManagement.Domain.csproj", "src/BallastLane.ProductManagement.Domain/"]
COPY ["src/BallastLane.ProductManagement.Application/BallastLane.ProductManagement.Application.csproj", "src/BallastLane.ProductManagement.Application/"]
COPY ["src/BallastLane.ProductManagement.Infrastructure/BallastLane.ProductManagement.Infrastructure.csproj", "src/BallastLane.ProductManagement.Infrastructure/"]
RUN dotnet restore "./src/BallastLane.ProductManagement.API/BallastLane.ProductManagement.API.csproj"
COPY . .
WORKDIR "/src/src/BallastLane.ProductManagement.API"
RUN dotnet build "./BallastLane.ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BallastLane.ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BallastLane.ProductManagement.API.dll"]
