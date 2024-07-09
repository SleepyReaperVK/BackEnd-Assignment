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
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app .

# Copy your configuration file (assuming it's in the root directory of your build context)
COPY config.yaml .

# Set the timezone (corrected spelling of Jerusalem and Israel)
ENV TZ=Asia/Jerusalem
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Set the entry point for the container
ENTRYPOINT ["dotnet", "KafkaProducer.dll"]
