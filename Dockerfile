FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5130

# Use the .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy the project files
COPY ["TestPusher.csproj", "./"]

# Restore dependencies
RUN dotnet restore "TestPusher.csproj"

# Copy the rest of the project files and build
COPY . .

WORKDIR "/src/"

RUN dotnet publish "TestPusher.csproj" -c Release -o /app/publish

RUN dotnet tool install --global Microsoft.Playwright.CLI

ENV PATH="$PATH:/root/.dotnet/tools"

RUN playwright install chromium

# RUN apt-get update && apt-get install -y wget gnupg2 apt-transport-https software-properties-common && \
#     wget -q "https://packages.microsoft.com/config/debian/10/prod.list" -O /etc/apt/sources.list.d/microsoft-prod.list && \
#     wget -q "https://packages.microsoft.com/keys/microsoft.asc" -O- | apt-key add - && \
#     apt-get update && apt-get install -y powershell

# RUN pwsh bin/Debug/net8.0/playwright.ps1 install  
# ENV PATH="$PATH:/root/.dotnet/tools"





# Final image
FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TestPusher.dll"]

