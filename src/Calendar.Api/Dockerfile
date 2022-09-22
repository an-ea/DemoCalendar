#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Calendar.Api/Calendar.Api.csproj", "Calendar.Api/"]
COPY ["Calendar.Application/Calendar.Application.csproj", "Calendar.Application/"]
COPY ["Calendar.Domain/Calendar.Domain.csproj", "Calendar.Domain/"]
COPY ["Calendar.Infrastructure/Calendar.Infrastructure.csproj", "Calendar.Infrastructure/"]
RUN dotnet restore "Calendar.Api/Calendar.Api.csproj"
COPY . .
WORKDIR "/src/Calendar.Api"
RUN dotnet build "Calendar.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Calendar.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calendar.Api.dll"]