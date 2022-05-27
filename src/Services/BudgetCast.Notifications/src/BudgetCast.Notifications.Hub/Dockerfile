# docker build . -t budgetcast/notificationshub --rm --build-arg USERNAME=appuser --no-cache --file .\Services\BudgetCast.Notifications\src\BudgetCast.Notifications.Hub\Dockerfile

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
COPY ["Common/BudgetCast.Common.Domain/BudgetCast.Common.Domain.csproj",            						"Common/BudgetCast.Common.Domain/"]
COPY ["Common/BudgetCast.Common.Application/BudgetCast.Common.Application.csproj",  						"Common/BudgetCast.Common.Application/"]
COPY ["Common/BudgetCast.Common.Application.Extensions/BudgetCast.Common.Application.Extensions.csproj",  	"Common/BudgetCast.Common.Application.Extensions/"]
COPY ["Common/BudgetCast.Common.Data/BudgetCast.Common.Data.csproj",            							"Common/BudgetCast.Common.Data/"]

COPY ["Common/BudgetCast.Common.Messaging.Abstractions/BudgetCast.Common.Messaging.Abstractions.csproj",                        "Common/BudgetCast.Common.Messaging.Abstractions/"]
COPY ["Common/BudgetCast.Common.Messaging.Azure.ServiceBus/BudgetCast.Common.Messaging.Azure.ServiceBus.csproj",                "Common/BudgetCast.Common.Messaging.Azure.ServiceBus/"]
COPY ["Common/BudgetCast.Common.Messaging.Azure.ServiceBus.Tests/BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.csproj",    "Common/BudgetCast.Common.Messaging.Azure.ServiceBus.Tests/"]

# integration with other services - in future libraries will be published as Nuget packages
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Messaging/BudgetCast.Expenses.Messaging.csproj", "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Messaging/"]

# application proj files
COPY ["Services/BudgetCast.Notifications/src/BudgetCast.Notifications.Hub/BudgetCast.Notifications.AppHub.csproj", "Services/BudgetCast.Notifications/src/BudgetCast.Notifications.Hub/"]

# tests proj files
COPY ["Services/BudgetCast.Notifications/tests/BudgetCast.Notifications.Tests.Unit/BudgetCast.Notifications.Tests.Unit.csproj", "Services/BudgetCast.Notifications/tests/BudgetCast.Notifications.Tests.Unit/"]

# sln
COPY ["Services/BudgetCast.Notifications/BudgetCast.Notifications.sln", "Services/BudgetCast.Notifications/BudgetCast.Notifications.sln"]

# restore (without arguments it uses .sln file by default - feature of .net core v3+)
# note: only runs if there's a !!!single!!! project or solution file in the folder
RUN dotnet restore "Services/BudgetCast.Notifications/BudgetCast.Notifications.sln"

# copy everything else
COPY ["Common/", "Common/"]
COPY ["Services/BudgetCast.Notifications/src", "Services/BudgetCast.Notifications/src"]

# integration with other services - in future libraries will be published as Nuget packages
COPY ["Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Messaging", "Services/BudgetCast.Expenses/src/BudgetCast.Expenses.Messaging"]

# build
WORKDIR /src/Services/BudgetCast.Notifications/src/BudgetCast.Notifications.Hub
RUN dotnet build "BudgetCast.Notifications.AppHub.csproj" -c $BUILD_CONFIG --no-restore -o /app/build

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
COPY ["Services/BudgetCast.Notifications/tests/", "Services/BudgetCast.Notifications/tests"]
WORKDIR /src/Services/BudgetCast.Notifications/tests/BudgetCast.Notifications.Tests.Unit

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
WORKDIR /src/Services/BudgetCast.Notifications/src/BudgetCast.Notifications.Hub
RUN dotnet publish "BudgetCast.Notifications.AppHub.csproj" -c $BUILD_CONFIG -o /app/publish --no-restore

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

ENTRYPOINT ["dotnet", "BudgetCast.Notifications.AppHub.dll"]