#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Challenges/Netrunner.Challenges.csproj", "Challenges/"]
COPY ["Shared/Netrunner.Shared.csproj", "Shared/"]
RUN dotnet restore "Challenges/Netrunner.Challenges.csproj"
COPY . .
WORKDIR "/src/Challenges"
RUN dotnet build "Netrunner.Challenges.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Netrunner.Challenges.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ./nginx/nginx.conf /etc/nginx/nginx.conf
COPY ./nginx/default.conf /etc/nginx/conf.d/default.conf