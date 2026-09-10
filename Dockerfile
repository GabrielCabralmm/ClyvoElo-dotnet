FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ClyvoElo.Api/ClyvoElo.Api.csproj ClyvoElo.Api/
RUN dotnet restore ClyvoElo.Api/ClyvoElo.Api.csproj
COPY ClyvoElo.Api/ ClyvoElo.Api/
WORKDIR /src/ClyvoElo.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClyvoElo.Api.dll"]
