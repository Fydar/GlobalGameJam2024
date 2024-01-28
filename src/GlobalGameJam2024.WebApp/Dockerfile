#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.1-alpine3.19-arm64v8 AS base
EXPOSE 8060
EXPOSE 8061
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
SHELL ["/bin/bash", "-c"]
ENV TERM=xterm-256color
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GlobalGameJam2024.WebApp/GlobalGameJam2024.WebApp.csproj", "GlobalGameJam2024.WebApp/"]
COPY ["GlobalGameJam2024.WebApp.Client/GlobalGameJam2024.WebApp.Client.csproj", "GlobalGameJam2024.WebApp.Client/"]
COPY ["GlobalGameJam2024.Simulation/GlobalGameJam2024.Simulation.csproj", "GlobalGameJam2024.Simulation/"]
RUN dotnet restore "./GlobalGameJam2024.WebApp/GlobalGameJam2024.WebApp.csproj"
COPY . .
WORKDIR "/src/GlobalGameJam2024.WebApp"
RUN dotnet build "./GlobalGameJam2024.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GlobalGameJam2024.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GlobalGameJam2024.WebApp.dll"]
