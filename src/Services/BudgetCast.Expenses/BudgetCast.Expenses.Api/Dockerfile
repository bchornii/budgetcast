#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BudgetCast.Expenses.Api/BudgetCast.Expenses.Api.csproj", "BudgetCast.Expenses.Api/"]
RUN dotnet restore "BudgetCast.Expenses.Api/BudgetCast.Expenses.Api.csproj"
COPY . .
WORKDIR "/src/BudgetCast.Expenses.Api"
RUN dotnet build "BudgetCast.Expenses.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BudgetCast.Expenses.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudgetCast.Expenses.Api.dll"]