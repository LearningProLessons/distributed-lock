FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# USER $APP_UID ← فعلاً کامنت چون تعریف نشده
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["distributed-cache.csproj", ""]
RUN dotnet restore "distributed-cache.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "distributed-cache.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "distributed-cache.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "distributed-cache.dll"]
