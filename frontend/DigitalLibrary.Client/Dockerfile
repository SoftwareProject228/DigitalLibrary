#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["frontend/DigitalLibrary.Client/DigitalLibrary.Client.csproj", "frontend/DigitalLibrary.Client/"]
RUN dotnet restore "frontend/DigitalLibrary.Client/DigitalLibrary.Client.csproj"
COPY . .
WORKDIR "/src/frontend/DigitalLibrary.Client"
RUN dotnet build "DigitalLibrary.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DigitalLibrary.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigitalLibrary.Client.dll"]