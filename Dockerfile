# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY VetCRM.Api/VetCRM.Api.csproj VetCRM.Api/
COPY VetCRM.SharedKernel/VetCRM.SharedKernel.csproj VetCRM.SharedKernel/
COPY VetCRM.Modules.Appointments/VetCRM.Modules.Appointments.csproj VetCRM.Modules.Appointments/
COPY VetCRM.Modules.Clients/VetCRM.Modules.Clients.csproj VetCRM.Modules.Clients/
COPY VetCRM.Modules.Identity/VetCRM.Modules.Identity.csproj VetCRM.Modules.Identity/
COPY VetCRM.Modules.MedicalRecords/VetCRM.Modules.MedicalRecords.csproj VetCRM.Modules.MedicalRecords/
COPY VetCRM.Modules.Notifications/VetCRM.Modules.Notifications.csproj VetCRM.Modules.Notifications/
COPY VetCRM.Modules.Pets/VetCRM.Modules.Pets.csproj VetCRM.Modules.Pets/
COPY VetCRM.Modules.Reports/VetCRM.Modules.Reports.csproj VetCRM.Modules.Reports/

RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet restore VetCRM.Api/VetCRM.Api.csproj

COPY VetCRM.Api/ VetCRM.Api/
COPY VetCRM.SharedKernel/ VetCRM.SharedKernel/
COPY VetCRM.Modules.Appointments/ VetCRM.Modules.Appointments/
COPY VetCRM.Modules.Clients/ VetCRM.Modules.Clients/
COPY VetCRM.Modules.Identity/ VetCRM.Modules.Identity/
COPY VetCRM.Modules.MedicalRecords/ VetCRM.Modules.MedicalRecords/
COPY VetCRM.Modules.Notifications/ VetCRM.Modules.Notifications/
COPY VetCRM.Modules.Pets/ VetCRM.Modules.Pets/
COPY VetCRM.Modules.Reports/ VetCRM.Modules.Reports/

RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet publish VetCRM.Api/VetCRM.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "VetCRM.Api.dll"]
