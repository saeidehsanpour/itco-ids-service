FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src
RUN apt update && apt install nuget -y
ADD ./nuget.config  ~/.nuget/NuGet/NuGet.Config

COPY . .
RUN dotnet restore --configfile=~/.nuget/NuGet/NuGet.Config "CaterSoft.IdentityServer/CaterSoft.IdentityServer.csproj"

WORKDIR "/src/CaterSoft.IdentityServer"
RUN dotnet build "CaterSoft.IdentityServer.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "CaterSoft.IdentityServer.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CaterSoft.IdentityServer.dll"]