# docker build -f 02_Consumer/Dockerfile -t vinicius1183/consumer-worker:v1.0.0 .
# docker push vinicius1183/consumer-worker:v1.0.0

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["02_Consumer/02_Consumer.csproj", "02_Consumer/"]
COPY ["03_Application/03_Application.csproj", "03_Application/"]
COPY ["04_Core/04_Core.csproj", "04_Core/"]
COPY ["05_Infraestructure/05_Infraestructure.csproj", "05_Infraestructure/"]
RUN dotnet nuget locals all --clear
RUN dotnet restore "02_Consumer/02_Consumer.csproj"

COPY . .
RUN dotnet build "02_Consumer/02_Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "02_Consumer/02_Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "02_Consumer.dll"]