
# Global configuration
ARG CONFIGURATION=Release

######################################### runtime stage #########################################
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
ARG USERNAME=bcuser
ARG USER_UID=1000
ARG USER_GID=$USER_UID
ARG PORT=80
WORKDIR /app
EXPOSE $PORT

######################################### build stage #########################################
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG CONFIGURATION
WORKDIR /src

# common proj files
COPY ["Common/BudgetCast.Common/BudgetCast.Common.csproj",                          "Common/BudgetCast.Common/"]
COPY ["Common/BudgetCast.Common.Web/BudgetCast.Common.Web.csproj",                  "Common/BudgetCast.Common.Web/"]
COPY ["Common/BudgetCast.Common.Application/BudgetCast.Common.Application.csproj",  "Common/BudgetCast.Common.Application/"]
COPY ["Common/BudgetCast.Common.Domain/BudgetCast.Common.Domain.csproj",            "Common/BudgetCast.Common.Domain/"]

COPY ["Common/BudgetCast.Common.Tests/BudgetCast.Common.Tests.csproj",									  "Common/BudgetCast.Common.Tests/"]
COPY ["Common/BudgetCast.Common.Application.Tests.Unit/BudgetCast.Common.Application.Tests.Unit.csproj",  "Common/BudgetCast.Common.Application.Tests.Unit/"]

# application proj files
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Api/BudgetCast.Expenses.Api.csproj",           "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Api/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Data/BudgetCast.Expenses.Data.csproj",         "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Data/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Queries/BudgetCast.Expenses.Queries.csproj",   "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Queries/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Domain/BudgetCast.Expenses.Domain.csproj",     "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Domain/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Commands/BudgetCast.Expenses.Commands.csproj", "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Commands/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Db/BudgetCast.Expenses.Db.sqlproj",			   "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Db/"]

# tests proj files
COPY ["Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit/BudgetCast.Expenses.Tests.Unit.csproj", "Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit/"]

# sln
COPY ["Services/BudgetCast.Expenses/BudgetCast.Expenses.sln", "Services/BudgetCast.Expenses/BudgetCast.Expenses.sln"]

# restore (without arguments it uses .sln file by default - feature of .net core v3+)
# note: only runs if there's a !!!single!!! project or solution file in the folder
RUN dotnet restore "Services/BudgetCast.Expenses/BudgetCast.Expenses.sln"

# copy everything source code
COPY ["Common/", "Common/"]
COPY ["Services/BudgetCast.Expenses/src/", "Services/BudgetCast.Expenses/src/"]

# build
RUN dotnet build "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Api/BudgetCast.Expenses.Api.csproj" -c $CONFIGURATION -o /app/build

######################################## test stage #########################################
FROM build AS test  
LABEL test=true
ARG CONFIGURATION
ARG REPGENVER="4.5.8"
ENV PATH="/root/.dotnet/tools:${PATH}"
WORKDIR /src

COPY ["Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit/BudgetCast.Expenses.Tests.Unit.csproj", "Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit/"]
RUN dotnet restore "/src/Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit/BudgetCast.Expenses.Tests.Unit.csproj"

COPY ["Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit/", "Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit/"]
WORKDIR /src/Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit/

RUN dotnet test -c $CONFIGURATION --collect:"XPlat Code Coverage" --settings ./codecoverage.runsettings --results-directory /testresults
RUN dotnet tool install --global dotnet-reportgenerator-globaltool --version $REPGENVER
RUN reportgenerator -reports:/testresults/**/coverage.cobertura.xml -targetdir:/testreport -reporttypes:Html

######################################### publish app stage #########################################
FROM build AS publish
ARG CONFIGURATION
WORKDIR /src
RUN dotnet publish "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Api/BudgetCast.Expenses.Api.csproj" -c $CONFIGURATION -o /app/publish --no-restore

######################################### runtime stage - final #########################################
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
RUN apk add --no-cache icu-libs
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8

# for Alpine linux
RUN apk add --no-cache shadow

# create group & user
RUN groupadd --gid $USER_GID $USERNAME \
	&& useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
	&& chown -R $USER_UID:$USER_GID /app

# switch to user
USER $USERNAME

ENTRYPOINT ["dotnet", "BudgetCast.Expenses.Api.dll"]