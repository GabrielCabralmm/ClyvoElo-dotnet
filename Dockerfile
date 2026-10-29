FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ClyvoElo.Domain/ClyvoElo.Domain.csproj ClyvoElo.Domain/
COPY ClyvoElo.Application/ClyvoElo.Application.csproj ClyvoElo.Application/
COPY ClyvoElo.Infrastructure/ClyvoElo.Infrastructure.csproj ClyvoElo.Infrastructure/
COPY ClyvoElo.Api/ClyvoElo.Api.csproj ClyvoElo.Api/
RUN dotnet restore ClyvoElo.Api/ClyvoElo.Api.csproj
COPY ClyvoElo.Domain/ ClyvoElo.Domain/
COPY ClyvoElo.Application/ ClyvoElo.Application/
COPY ClyvoElo.Infrastructure/ ClyvoElo.Infrastructure/
COPY ClyvoElo.Api/ ClyvoElo.Api/
RUN dotnet publish ClyvoElo.Api/ClyvoElo.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClyvoElo.Api.dll"]
