
# docker build . -t budgetcast/expensesapi --rm --build-arg USERNAME=appuser --no-cache --file .\Services\BudgetCast.Expenses\src\BudgetCast.Expenses.Api\Dockerfile

# Global configuration
ARG BUILD_CONFIG=Release
ARG REPORT_VER="4.5.8"
ARG TEST_ON_BUILD=true

######################################### runtime stage #########################################
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
ARG USERNAME=bcuser
ARG USER_UID=1000
ARG USER_GID=$USER_UID
ARG PORT=80
WORKDIR /app
EXPOSE $PORT

RUN apk add --no-cache \
    icu-libs=67.1-r2 \
    shadow=4.8.1-r0 \
    tzdata=2022a-r0

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8

######################################### build stage #########################################
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIG
WORKDIR /src

# common proj files
COPY ["Common/BudgetCast.Common/BudgetCast.Common.csproj",                          						"Common/BudgetCast.Common/"]
COPY ["Common/BudgetCast.Common.Web/BudgetCast.Common.Web.csproj",                  						"Common/BudgetCast.Common.Web/"]
COPY ["Common/BudgetCast.Common.Application/BudgetCast.Common.Application.csproj",  						"Common/BudgetCast.Common.Application/"]
COPY ["Common/BudgetCast.Common.Application.Extensions/BudgetCast.Common.Application.Extensions.csproj",  	"Common/BudgetCast.Common.Application.Extensions/"]
COPY ["Common/BudgetCast.Common.Domain/BudgetCast.Common.Domain.csproj",            						"Common/BudgetCast.Common.Domain/"]
COPY ["Common/BudgetCast.Common.Data/BudgetCast.Common.Data.csproj",            							"Common/BudgetCast.Common.Data/"]

COPY ["Common/BudgetCast.Common.Tests/BudgetCast.Common.Tests.csproj",									  	"Common/BudgetCast.Common.Tests/"]
COPY ["Common/BudgetCast.Common.Application.Tests.Unit/BudgetCast.Common.Application.Tests.Unit.csproj",  	"Common/BudgetCast.Common.Application.Tests.Unit/"]
COPY ["Common/BudgetCast.Common.Web.Tests/BudgetCast.Common.Web.Tests.csproj",  							"Common/BudgetCast.Common.Web.Tests/"]
COPY ["Common/BudgetCast.Common.Data.Tests.Unit/BudgetCast.Common.Data.Tests.Unit.csproj",  				"Common/BudgetCast.Common.Data.Tests.Unit/"]

COPY ["Common/BudgetCast.Common.Messaging.Abstractions/BudgetCast.Common.Messaging.Abstractions.csproj",            "Common/BudgetCast.Common.Messaging.Abstractions/"]
COPY ["Common/BudgetCast.Common.Messaging.Azure.ServiceBus/BudgetCast.Common.Messaging.Azure.ServiceBus.csproj",    "Common/BudgetCast.Common.Messaging.Azure.ServiceBus/"]

# application proj files
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Api/BudgetCast.Expenses.Api.csproj",                "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Api/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Data/BudgetCast.Expenses.Data.csproj",              "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Data/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Queries/BudgetCast.Expenses.Queries.csproj",        "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Queries/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Domain/BudgetCast.Expenses.Domain.csproj",          "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Domain/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Commands/BudgetCast.Expenses.Commands.csproj",      "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Commands/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Db/BudgetCast.Expenses.Db.sqlproj",			        "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Db/"]
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Messaging/BudgetCast.Expenses.Messaging.csproj",    "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Messaging/"]

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
WORKDIR /src/Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Api
RUN dotnet build "BudgetCast.Expenses.Api.csproj" -c $BUILD_CONFIG --no-restore -o /app/build

######################################## test stage #########################################
FROM build AS test  
LABEL test=true
ARG BUILD_CONFIG
ARG REPORT_VER
ARG TEST_ON_BUILD
ENV PATH="/root/.dotnet/tools:${PATH}"
ENV RUN_CONFIG=${BUILD_CONFIG}
WORKDIR /src

# copy tests
COPY ["Services/BudgetCast.Expenses/tests/", "Services/BudgetCast.Expenses/tests/"]
WORKDIR /src/Services/BudgetCast.Expenses/tests/BudgetCast.Expenses.Tests.Unit

# install report generator
RUN dotnet tool install --global dotnet-reportgenerator-globaltool --version ${REPORT_VER}

# run tests as a part of build process
RUN if [ -z "$TEST_ON_BUILD" ]; then \
        echo 'Running tests during image build has been skipped'; \
    else \
        dotnet test -c $BUILD_CONFIG --no-restore --collect:"XPlat Code Coverage" --settings ./codecoverage.runsettings --results-directory /tests/unit/testresults; \
    fi

# for ci pipeline to execute test container image and retrieve test results
ENTRYPOINT \
    dotnet test -c $RUN_CONFIG --collect:"XPlat Code Coverage" --settings ./codecoverage.runsettings --results-directory /tests/unit/testresults; \
    reportgenerator -reports:/tests/unit/testresults/**/coverage.cobertura.xml -targetdir:/tests/unit/testreport -reporttypes:Html;

######################################### publish app stage #########################################
FROM build AS publish
ARG BUILD_CONFIG
WORKDIR /src/Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Api
RUN dotnet publish "BudgetCast.Expenses.Api.csproj" -c $BUILD_CONFIG -o /app/publish --no-restore

######################################### runtime stage - final #########################################
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# create group & user
RUN groupadd --gid $USER_GID $USERNAME \
	&& useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
	&& chown -R $USER_UID:$USER_GID /app \
    && chmod -R 550 /app

# switch to user
USER $USERNAME

ENTRYPOINT ["dotnet", "BudgetCast.Expenses.Api.dll"]