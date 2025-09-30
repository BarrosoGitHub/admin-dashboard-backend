FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Add private NuGet source
ARG NUGET_PASSWORD
RUN dotnet nuget add source http://172.16.50.65:8084/repository/nuget-private/ -n private -u admin -p $NUGET_PASSWORD --allow-insecure-connections

# Copy project files first
COPY OPTConfigurator.csproj .

# Restore dependencies
RUN dotnet restore "OPTConfigurator.csproj"

# Copy source code
COPY . .

RUN dotnet publish "OPTConfigurator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim-arm64v8 AS final
RUN apt-get update && \
    apt-get install -y libnm0 libreadline-dev dnsmasq bash network-manager && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
ENV DOTNET_ROOT=/usr/share/dotnet

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "OPTConfigurator.dll"]
