#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Lider-V-APIServices/Lider-V-APIServices.csproj", "Lider-V-APIServices/"]
RUN dotnet restore "Lider-V-APIServices/Lider-V-APIServices.csproj"
COPY . .
WORKDIR "/src/Lider-V-APIServices"
RUN dotnet build "Lider-V-APIServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lider-V-APIServices.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lider-V-APIServices.dll"]