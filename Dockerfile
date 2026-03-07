FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish FlowBoard.API/FlowBoard.API.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
CMD ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet FlowBoard.API.dll