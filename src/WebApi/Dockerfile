FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . .

FROM build AS publish
WORKDIR /src/WebApi
RUN dotnet publish "WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN cp /app/appsettings-pruebas.json /app/appsettings.json
ENTRYPOINT ["dotnet", "WebApi.dll"]
