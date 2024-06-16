FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS development
COPY . /AppDev
WORKDIR /AppDev/WarehouseManagement.Api
CMD dotnet run --no-launch-profile

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "WarehouseManagement.Api.dll"]