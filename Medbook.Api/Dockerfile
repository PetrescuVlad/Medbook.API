# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["Medbook.Api.csproj", "./"]
RUN dotnet restore "Medbook.Api.csproj"

# Copy everything and publish
COPY . .
RUN dotnet publish "Medbook.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# App listens on 8080 in container
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Medbook.Api.dll"]
