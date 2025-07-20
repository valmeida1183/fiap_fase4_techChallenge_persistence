# docker build -f 01_WebApi/Dockerfile -t vinicius1183/persistance-api:v1.0.1 .
# docker push vinicius1183/persistance-api:v1.0.1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["01_WebApi/01_WebApi.csproj", "01_WebApi/"]
COPY ["03_Application/03_Application.csproj", "03_Application/"]
COPY ["04_Core/04_Core.csproj", "04_Core/"]
COPY ["05_Infraestructure/05_Infraestructure.csproj", "05_Infraestructure/"]
RUN dotnet nuget locals all --clear
RUN dotnet restore "01_WebApi/01_WebApi.csproj" 

COPY . .
RUN dotnet build "01_WebApi/01_WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "01_WebApi/01_WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "01_WebApi.dll"]