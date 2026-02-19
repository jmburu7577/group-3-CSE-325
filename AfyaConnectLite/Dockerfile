# Use the official .NET 8.0 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Clear any existing NuGet cache
RUN dotnet nuget locals all --clear

# Copy the csproj and restore as distinct layers
COPY ["AfyaConnectLite.csproj", "."]
RUN dotnet restore "./AfyaConnectLite.csproj"

# Copy the rest of the files and build
COPY . .
RUN dotnet build "AfyaConnectLite.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AfyaConnectLite.csproj" -c Release -o /app/publish

# Build the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create the database directory and set permissions
RUN mkdir -p /app/data && chmod 777 /app/data

# Set the connection string to use the data directory
ENV ASPNETCORE_ConnectionStrings__DefaultConnection="Data Source=/app/data/afyaconnect.db"

# Start the application
ENTRYPOINT ["dotnet", "AfyaConnectLite.dll"]
