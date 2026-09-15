FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY MaintenanceDesk.Api/MaintenanceDesk.Api.csproj MaintenanceDesk.Api/
RUN dotnet restore MaintenanceDesk.Api/MaintenanceDesk.Api.csproj
COPY MaintenanceDesk.Api/ MaintenanceDesk.Api/
RUN dotnet publish MaintenanceDesk.Api/MaintenanceDesk.Api.csproj --configuration Release --output /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MaintenanceDesk.Api.dll"]