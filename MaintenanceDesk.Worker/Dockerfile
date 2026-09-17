FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY MaintenanceDesk.Worker/MaintenanceDesk.Worker.csproj MaintenanceDesk.Worker/
RUN dotnet restore MaintenanceDesk.Worker/MaintenanceDesk.Worker.csproj
COPY MaintenanceDesk.Worker/ MaintenanceDesk.Worker/
RUN dotnet publish MaintenanceDesk.Worker/MaintenanceDesk.Worker.csproj --configuration Release --output /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "MaintenanceDesk.Worker.dll"]