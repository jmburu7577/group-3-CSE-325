FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore first for better layer caching
COPY AfyaConnectLite/AfyaConnectLite.csproj AfyaConnectLite/
RUN dotnet restore AfyaConnectLite/AfyaConnectLite.csproj

# Copy the rest of the sources and publish
COPY . .
RUN dotnet publish AfyaConnectLite/AfyaConnectLite.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "AfyaConnectLite.dll"]
