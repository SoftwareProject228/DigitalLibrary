FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY DigitalLibraryWebApp/DigitalLibraryWebApp.csproj DigitalLibraryWebApp/
RUN dotnet restore DigitalLibraryWebApp/DigitalLibraryWebApp.csproj

# Copy everything else and build
COPY . .
WORKDIR /app/DigitalLibraryWebApp
RUN dotnet build DigitalLibraryWebApp.csproj -c Release -o /out
RUN dotnet publish DigitalLibraryWebApp.csproj -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 80
WORKDIR /app
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "DigitalLibraryWebApp.dll"]
