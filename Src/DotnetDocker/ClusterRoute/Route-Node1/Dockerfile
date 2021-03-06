#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["ClusterRoute/Route-Node1/Route-Node1.csproj", "ClusterRoute/Route-Node1/"]
COPY ["SingletonAkka/Shard/Common/Common.csproj", "SingletonAkka/Shard/Common/"]
COPY ["ShardNode/ShardNode.csproj", "ShardNode/"]
RUN dotnet restore "ClusterRoute/Route-Node1/Route-Node1.csproj"
COPY . .
WORKDIR "/src/ClusterRoute/Route-Node1"
RUN dotnet build "Route-Node1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Route-Node1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Route-Node1.dll"]