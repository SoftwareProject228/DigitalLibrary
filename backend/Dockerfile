FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY DigitalLibrary/DigitalLibrary.csproj DigitalLibrary/
RUN dotnet restore DigitalLibrary/DigitalLibrary.csproj

# Copy everything else and build
COPY . .
WORKDIR /app/DigitalLibrary
RUN dotnet build DigitalLibrary.csproj -c Release -o /out
RUN dotnet publish DigitalLibrary.csproj -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 80
WORKDIR /app
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "DigitalLibrary.dll"]
