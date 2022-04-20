FROM mcr.microsoft.com/dotnet/runtime:5.0-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["K8sDemoLogManager/K8sDemoLogManager.csproj", "K8sDemoLogManager/"]
RUN dotnet restore "K8sDemoLogManager/K8sDemoLogManager.csproj"
COPY . .
WORKDIR "/src/K8sDemoLogManager"
RUN dotnet build "K8sDemoLogManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "K8sDemoLogManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K8sDemoLogManager.dll"]
