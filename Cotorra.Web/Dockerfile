#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.6-buster-slim AS base
WORKDIR /app
EXPOSE 80

RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.6-buster-slim AS build
WORKDIR /src
COPY ["Cotorra.Web/Cotorra.Web.csproj", "Cotorra.Web/"]
COPY ["Cotorra.Schema/Cotorra.Schema.csproj", "Cotorra.Schema/"]
COPY ["Cotorra.ClientProxy/Cotorra.ClientProxy.csproj", "Cotorra.ClientProxy/"]
COPY ["Cotorra.Client/Cotorra.Client.csproj", "Cotorra.Client/"]
COPY ["Cotorra.Core/Cotorra.Core.csproj", "Cotorra.Core/"]
COPY ["Cotorra.ExpressionParser.Core/Cotorra.ExpressionParser.Core.csproj", "Cotorra.ExpressionParser.Core/"]
COPY ["Cotorra.ClientLocal/Cotorra.ClientLocal.csproj", "Cotorra.ClientLocal/"]

RUN dotnet restore "Cotorra.Web/Cotorra.Web.csproj"
COPY . .
WORKDIR "/src/Cotorra.Web"
RUN dotnet build "Cotorra.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cotorra.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cotorra.Web.dll"]