FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ApiCentralDocsWeb.csproj", "./"]
RUN dotnet restore "ApiCentralDocsWeb.csproj"

COPY . .
RUN dotnet publish "ApiCentralDocsWeb.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

ENTRYPOINT ["dotnet", "ApiCentralDocsWeb.dll"]