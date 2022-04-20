FROM mcr.microsoft.com/dotnet/runtime:5.0-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["K8sDemoWorker/K8sDemoWorker.csproj", "K8sDemoWorker/"]
RUN dotnet restore "K8sDemoWorker/K8sDemoWorker.csproj"
COPY . .
WORKDIR "/src/K8sDemoWorker"
RUN dotnet build "K8sDemoWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "K8sDemoWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K8sDemoWorker.dll"]
