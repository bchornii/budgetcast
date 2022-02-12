# docker build . -t budgetcast/notificationshub --rm --build-arg USERNAME=appuser --no-cache --file .\Services\BudgetCast.Notifications\src\BudgetCast.Notifications.Hub\Dockerfile

# Global configuration
ARG CONFIGURATION=Release

######################################### runtime stage #########################################
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
ARG USERNAME=bcuser
ARG USER_UID=1000
ARG USER_GID=$USER_UID
ARG PORT=80
ARG TLS_PORT=443
WORKDIR /app
EXPOSE $PORT
EXPOSE $TLS_PORT

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false
RUN apk add --no-cache icu-libs
ENV LC_ALL en_US.UTF-8
ENV LANG en_US.UTF-8

######################################### build stage #########################################
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG CONFIGURATION
WORKDIR /src

# common proj files
COPY ["Common/BudgetCast.Common/BudgetCast.Common.csproj",                          "Common/BudgetCast.Common/"]
COPY ["Common/BudgetCast.Common.Web/BudgetCast.Common.Web.csproj",                  "Common/BudgetCast.Common.Web/"]
COPY ["Common/BudgetCast.Common.Application/BudgetCast.Common.Application.csproj",  "Common/BudgetCast.Common.Application/"]
COPY ["Common/BudgetCast.Common.Domain/BudgetCast.Common.Domain.csproj",            "Common/BudgetCast.Common.Domain/"]

# application proj files
COPY ["Services/BudgetCast.Notifications/src/BudgetCast.Notifications.Hub/BudgetCast.Notifications.AppHub.csproj", "Services/BudgetCast.Notifications/src/BudgetCast.Notifications.Hub/"]

# sln
COPY ["Services/BudgetCast.Notifications/BudgetCast.Notifications.sln", "Services/BudgetCast.Notifications/BudgetCast.Notifications.sln"]

# restore (without arguments it uses .sln file by default - feature of .net core v3+)
# note: only runs if there's a !!!single!!! project or solution file in the folder
RUN dotnet restore "Services/BudgetCast.Notifications/BudgetCast.Notifications.sln"

# copy everything else
COPY ["Common/", "Common/"]
COPY ["Services/BudgetCast.Notifications/src", "Services/BudgetCast.Notifications/src"]

# build
RUN dotnet build "Services/BudgetCast.Notifications/src/BudgetCast.Notifications.Hub/BudgetCast.Notifications.AppHub.csproj" -c $CONFIGURATION -o /app/build

######################################## test stage #########################################

# no tests so far

######################################### publish app stage #########################################

FROM build AS publish
ARG CONFIGURATION
WORKDIR /src
RUN dotnet publish "Services/BudgetCast.Notifications/src/BudgetCast.Notifications.Hub/BudgetCast.Notifications.AppHub.csproj" -c $CONFIGURATION -o /app/publish --no-restore

######################################### runtime stage - final #########################################
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# for Alpine linux
RUN apk add --no-cache shadow

# create group & user
RUN groupadd --gid $USER_GID $USERNAME \
	&& useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
	&& chown -R $USER_UID:$USER_GID /app

# switch to user
USER $USERNAME

ENTRYPOINT ["dotnet", "BudgetCast.Notifications.AppHub.dll"]