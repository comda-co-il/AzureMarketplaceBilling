# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file
COPY ["CcmsCommercialPlatform/CcmsCommercialPlatform.csproj", "CcmsCommercialPlatform/"]

# Restore dependencies
RUN dotnet restore "CcmsCommercialPlatform/CcmsCommercialPlatform.csproj"

# Copy the rest of the source
COPY . .

# Build the backend (skip client app build as it's done separately in nodebuild stage)
WORKDIR "/src/CcmsCommercialPlatform"
RUN dotnet build "CcmsCommercialPlatform.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:SkipClientAppBuild=true

# ---- React/Vite build step ----
FROM node:20 AS nodebuild
WORKDIR /client
COPY ./CcmsCommercialPlatform/ClientApp ./ClientApp
WORKDIR /client/ClientApp
RUN npm install
RUN npm run build

# ---- Publish .NET backend and include React output ----
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "CcmsCommercialPlatform.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:SkipClientAppBuild=true

# Copy built React files into wwwroot
COPY --from=nodebuild /client/ClientApp/dist /app/publish/wwwroot

# ---- Final runtime image ----
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS="http://+:8080"
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CcmsCommercialPlatform.dll"]
