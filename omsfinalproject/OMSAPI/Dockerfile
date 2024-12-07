# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8000
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OMSAPI/OMSAPI.csproj", "OMSAPI/"]
RUN dotnet restore "OMSAPI/OMSAPI.csproj"
COPY . .
WORKDIR "/src/OMSAPI"
RUN dotnet build "OMSAPI.csproj" -c Release -o /app/build

Run dotnet add package MongoDB.Driver
Run dotnet add package MongoDB.Bson
Run dotnet add package Serilog.AspNetCore
Run dotnet add package Serilog.Sinks.File
Run dotnet add package Microsoft.IdentityModel.Tokens
Run dotnet add package System.IdentityModel.Tokens.Jwt
Run dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer -v 6.0.0
Run dotnet add package Swashbuckle.AspNetCore.Filters
Run dotnet add package Newtonsoft.Json
Run dotnet add package DateOnlyTimeOnly.AspNet.Converters
Run dotnet add package Swashbuckle.AspNetCore.Annotations
Run dotnet add package MailKit
Run dotnet add RazorLight
Run dotnet add Razor.Templating.Core


FROM build AS publish
RUN dotnet publish "OMSAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OMSAPI.dll"]

