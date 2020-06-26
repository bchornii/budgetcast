# to leverage FastMode debug in VS
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# copy .csproj to leverage layer caching
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["*.sln", "."]
COPY ["BudgetCast.Dashboard.Api/BudgetCast.Dashboard.Api.csproj", "BudgetCast.Dashboard.Api/"]
COPY ["BudgetCast.Dashboard.Data/BudgetCast.Dashboard.Data.csproj", "BudgetCast.Dashboard.Data/"]
COPY ["BudgetCast.Dashboard.Domain/BudgetCast.Dashboard.Domain.csproj", "BudgetCast.Dashboard.Domain/"]
COPY ["BudgetCast.Dashboard.ReadAccessors/BudgetCast.Dashboard.ReadAccessors.csproj", "BudgetCast.Dashboard.ReadAccessors/"]
COPY ["BudgetCast.Dashboard.Commands/BudgetCast.Dashboard.Commands.csproj", "BudgetCast.Dashboard.Commands/"]
COPY ["BudgetCast.Dashboard.WriteAccessors/BudgetCast.Dashboard.WriteAccessors.csproj", "BudgetCast.Dashboard.WriteAccessors/"]
COPY ["BudgetCast.Dashboard.Repository/BudgetCast.Dashboard.Repository.csproj", "BudgetCast.Dashboard.Repository/"]
COPY ["BudgetCast.Dashboard.Queries/BudgetCast.Dashboard.Queries.csproj", "BudgetCast.Dashboard.Queries/"]

# restore (without arguments it uses .sln file by default - feature of .net core v3+)
# note: only runs if there's a !!!single!!! project or solution file in the folder
RUN dotnet restore

# copy everything else
COPY . .
WORKDIR "/src/BudgetCast.Dashboard.Api"

# publish
FROM build AS publish
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
RUN apk add --no-cache icu-libs
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8

ENTRYPOINT ["dotnet", "BudgetCast.Dashboard.Api.dll"]