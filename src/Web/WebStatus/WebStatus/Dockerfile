# to leverage FastMode debug in VS
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WebStatus/WebStatus.csproj", "WebStatus/"]

# restore packages
RUN dotnet restore "WebStatus/WebStatus.csproj"

# copy rest of files
COPY . .
WORKDIR "/src/WebStatus"

# build
RUN dotnet build "WebStatus.csproj" -c Release -o /app/build

# publish
FROM build AS publish
RUN dotnet publish "WebStatus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebStatus.dll"]