ARG NODE_IMAGE=node:12.18

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${NODE_IMAGE} as node-build
WORKDIR /web

# install and cache app dependencies
COPY ["BudgetCast.Spa/Client/package.json", "."]
COPY ["BudgetCast.Spa/Client/package-lock.json", "."]
RUN npm install

COPY ["BudgetCast.Spa/Client", "."]
RUN npm run build:prod

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["*.sln", "."]
COPY ["BudgetCast.Spa/BudgetCast.Spa.csproj", "BudgetCast.Spa/"]

# restore
RUN dotnet restore

# copy everything else
COPY . .
COPY --from=node-build /web/dist/budgetcast /src/BudgetCast.Spa/wwwroot/
WORKDIR "/src/BudgetCast.Spa"

# publish
FROM build AS publish
RUN dotnet publish "BudgetCast.Spa.csproj" -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BudgetCast.Spa.dll"]