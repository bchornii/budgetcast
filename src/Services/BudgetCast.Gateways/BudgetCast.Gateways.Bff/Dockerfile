FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BudgetCast.Gateways.Bff/BudgetCast.Gateways.Bff.csproj", "BudgetCast.Gateways.Bff/"]
RUN dotnet restore "BudgetCast.Gateways.Bff/BudgetCast.Gateways.Bff.csproj"
COPY . .
WORKDIR "/src/BudgetCast.Gateways.Bff"
RUN dotnet build "BudgetCast.Gateways.Bff.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BudgetCast.Gateways.Bff.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudgetCast.Gateways.Bff.dll"]
