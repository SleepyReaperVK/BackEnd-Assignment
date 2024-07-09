# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY KafkaConsumer.csproj .
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Build the application
RUN dotnet publish -c Release -o /app

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy the published application
COPY --from=build /app .

# Copy configuration file (config.yaml)
COPY config.yaml .

# Set the entry point
ENTRYPOINT ["dotnet", "KafkaConsumer.dll"]
