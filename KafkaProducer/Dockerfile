# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY KafkaProducer.csproj .
RUN dotnet restore

# Copy the remaining source code and build the application
COPY . .
RUN dotnet publish -c Release -o /app

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
COPY config.yaml .

ENTRYPOINT ["dotnet", "KafkaProducer.dll"]
