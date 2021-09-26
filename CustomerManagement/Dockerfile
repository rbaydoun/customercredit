#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["CustomerManagement/CustomerManagement.csproj", "CustomerManagement/"]
RUN dotnet restore "CustomerManagement/CustomerManagement.csproj"
COPY . .
WORKDIR "/src/CustomerManagement"
RUN dotnet build "CustomerManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CustomerManagement.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomerManagement.dll"]
