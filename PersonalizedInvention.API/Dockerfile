# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files
COPY PersonalizedInvention.Domain/PersonalizedInvention.Domain.csproj PersonalizedInvention.Domain/
COPY PersonalizedInvention.Application/PersonalizedInvention.Application.csproj PersonalizedInvention.Application/
COPY PersonalizedInvention.Infrastructure/PersonalizedInvention.Infrastructure.csproj PersonalizedInvention.Infrastructure/
COPY PersonalizedInvention.API/PersonalizedInvention.API.csproj PersonalizedInvention.API/

# Restore dependencies
RUN dotnet restore PersonalizedInvention.API/PersonalizedInvention.API.csproj

# Copy source code
COPY . .

# Build and publish
RUN dotnet publish PersonalizedInvention.API/PersonalizedInvention.API.csproj \
    -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PersonalizedInvention.API.dll"]