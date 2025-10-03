FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["PodPeek.Api/PodPeek.Api.csproj", "PodPeek.Api/"]
COPY ["PodPeek.Domain/PodPeek.Domain.csproj", "PodPeek.Domain/"]
COPY ["PodPeek.KubeClient/PodPeek.KubeClient.csproj", "PodPeek.KubeClient/"]

RUN dotnet restore "PodPeek.Api/PodPeek.Api.csproj"
COPY . .

WORKDIR "/src/PodPeek.Api"
RUN dotnet build "PodPeek.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:UseAppHost=false
RUN dotnet publish "PodPeek.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

