FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["AirportRouteApi/AirportRouteApi.csproj", "AirportRouteApi/"]
RUN dotnet restore "AirportRouteApi/AirportRouteApi.csproj"
COPY . .
WORKDIR "/src/AirportRouteApi"
RUN dotnet build "AirportRouteApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AirportRouteApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AirportRouteApi.dll"]