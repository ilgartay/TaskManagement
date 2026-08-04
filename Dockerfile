FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TaskManagement.API.csproj ./
RUN dotnet restore TaskManagement.API.csproj

COPY . ./
RUN dotnet publish TaskManagement.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

RUN mkdir -p /app/uploads && chown -R app:app /app/uploads
USER app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]
