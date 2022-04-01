FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 5001

ENV ASPNETCORE_URLS=https://+:5001

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["K8sDemoHubManager/K8sDemoHubManager.csproj", "K8sDemoHubManager/"]
RUN dotnet restore "K8sDemoHubManager/K8sDemoHubManager.csproj"
COPY . .
WORKDIR "/src/K8sDemoHubManager"
RUN dotnet build "K8sDemoHubManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "K8sDemoHubManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K8sDemoHubManager.dll"]
