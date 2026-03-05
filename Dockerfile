FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/NotificationsAPI/NotificationsAPI.csproj", "src/NotificationsAPI/"]
COPY ["src/NotificationsAPI.Application/NotificationsAPI.Application.csproj", "src/NotificationsAPI.Application/"]
COPY ["src/NotificationsAPI.Domain/NotificationsAPI.Domain.csproj", "src/NotificationsAPI.Domain/"]
COPY ["src/NotificationsAPI.Infrastructure/NotificationsAPI.Infrastructure.csproj", "src/NotificationsAPI.Infrastructure/"]
RUN dotnet restore "src/NotificationsAPI/NotificationsAPI.csproj"
COPY . .
WORKDIR "/src/src/NotificationsAPI"
RUN dotnet build "NotificationsAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationsAPI.dll"]