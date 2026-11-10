FROM mcr.microsoft.com/dotnet/sdk:9.0 as build
WORKDIR /src
COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

ENV DOTNET_hostBuilder__reloadConfigOnChange=false

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT [ "dotnet", "backend.dll" ]
