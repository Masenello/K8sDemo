FROM mcr.microsoft.com/dotnet/runtime:5.0-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["K8sDemoDirector/K8sDemoDirector.csproj", "K8sDemoDirector/"]
RUN dotnet restore "K8sDemoDirector/K8sDemoDirector.csproj"
COPY . .
WORKDIR "/src/K8sDemoDirector"
RUN dotnet build "K8sDemoDirector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "K8sDemoDirector.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K8sDemoDirector.dll"]
