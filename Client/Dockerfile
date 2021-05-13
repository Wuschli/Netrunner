#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Client/Netrunner.Client.csproj", "Client/"]
COPY ["Shared/Netrunner.Shared.csproj", "Shared/"]
RUN dotnet restore "Client/Netrunner.Client.csproj"
COPY . .
WORKDIR "/src/Client"
RUN dotnet build "Netrunner.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Netrunner.Client.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ./nginx/nginx.conf /etc/nginx/nginx.conf
COPY ./nginx/default.conf /etc/nginx/conf.d/default.conf