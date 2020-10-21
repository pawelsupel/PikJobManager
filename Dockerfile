FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY PikJobManager/. ./
RUN ls
RUN dotnet restore PikJobManager.sln
RUN dotnet build PikJobManagerModules/PikJobManagerModules.csproj -c Release
RUN dotnet build PikJobManager.App/PikJobManager.App.csproj -c Release

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY PikJobManager/Build .
ENTRYPOINT ["dotnet", "PikJobManager.App.dll"]