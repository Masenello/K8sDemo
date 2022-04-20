#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5501

ENV ASPNETCORE_URLS=https://+:5501


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["K8sDemoApi/K8sDemoApi.csproj", "K8sDemoApi/"]
RUN dotnet restore "K8sDemoApi/K8sDemoApi.csproj"
COPY . .
WORKDIR "/src/K8sDemoApi"
RUN dotnet build "K8sDemoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "K8sDemoApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K8sDemoApi.dll"]