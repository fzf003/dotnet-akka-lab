#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

ENV CLUSTER_SEEDS "[]"
ENV CLUSTER_IP ""
ENV CLUSTER_PORT "4053"

ENV 19600 4053



FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["SeedNode/SeedNode.csproj", "SeedNode/"]
COPY ["ShardNode/ShardNode.csproj", "ShardNode/"]
RUN dotnet restore "SeedNode/SeedNode.csproj"
COPY . .
WORKDIR "/src/SeedNode"
RUN dotnet build "SeedNode.csproj" -c Release -o /app/build

RUN dotnet tool install --global pbm 

FROM build AS publish
RUN dotnet publish "SeedNode.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV PATH="${PATH}:/root/.dotnet/tools"
ENTRYPOINT ["dotnet", "SeedNode.dll"]