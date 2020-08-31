#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster-slim AS build
WORKDIR /src
COPY ["Cotorra.WebAPI/Cotorra.WebAPI.csproj", "Cotorra.WebAPI/"]
COPY ["Cotorra.General.Core/Cotorra.General.Core.csproj", "Cotorra.General.Core/"]
COPY ["Cotorra.General.Schema/Cotorra.General.Schema.csproj", "Cotorra.General.Schema/"]
COPY ["Cotorra.Core/Cotorra.Core.csproj", "Cotorra.Core/"]
COPY ["Cotorra.Schema/Cotorra.Schema.csproj", "Cotorra.Schema/"]
COPY ["Cotorra.ExpressionParser.Core/Cotorra.ExpressionParser.Core.csproj", "Cotorra.ExpressionParser.Core/"]
RUN dotnet restore "Cotorra.WebAPI/Cotorra.WebAPI.csproj"
COPY . .
WORKDIR "/src/Cotorra.WebAPI"
RUN dotnet build "Cotorra.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cotorra.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cotorra.WebAPI.dll"]
