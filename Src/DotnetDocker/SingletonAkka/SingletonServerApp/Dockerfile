#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

ENV CLUSTER_SEEDS "[]"
ENV CLUSTER_IP ""
ENV CLUSTER_PORT "6055"

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["SingletonAkka/SingletonServerApp/SingletonServerApp.csproj", "SingletonAkka/SingletonServerApp/"]
COPY ["SingletonAkka/Shard/Common/Common.csproj", "SingletonAkka/Shard/Common/"]
RUN dotnet restore "SingletonAkka/SingletonServerApp/SingletonServerApp.csproj"
COPY . .
WORKDIR "/src/SingletonAkka/SingletonServerApp"
RUN dotnet build "SingletonServerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SingletonServerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SingletonServerApp.dll"]