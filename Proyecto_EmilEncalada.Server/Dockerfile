FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Proyecto_EmilEncalada.Server.csproj", "./"]
RUN dotnet restore "./Proyecto_EmilEncalada.Server.csproj"

COPY . .
RUN dotnet publish "Proyecto_EmilEncalada.Server.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Proyecto_EmilEncalada.Server.dll"];
