#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

ENV CLUSTER_SEEDS "[]"
ENV CLUSTER_IP ""
ENV CLUSTER_PORT "4053"

ENV 19600 4053

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["AkkaTcpServer/AkkaTcpServer.csproj", "AkkaTcpServer/"]
COPY ["SingletonAkka/Shard/Common/Common.csproj", "SingletonAkka/Shard/Common/"]
COPY ["ShardNode/ShardNode.csproj", "ShardNode/"]
RUN dotnet restore "AkkaTcpServer/AkkaTcpServer.csproj"
COPY . .
WORKDIR "/src/AkkaTcpServer"
RUN dotnet build "AkkaTcpServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AkkaTcpServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AkkaTcpServer.dll"]