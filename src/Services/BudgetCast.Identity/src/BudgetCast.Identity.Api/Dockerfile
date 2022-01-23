
# docker build . -t budgetcast/identityapi --rm --build-arg USERNAME=appuser --no-cache --file .\Services\BudgetCast.Identity\src\BudgetCast.Identity.Api\Dockerfile

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

# application proj files
COPY ["Services/BudgetCast.Identity/src/BudgetCast.Identity.Api/BudgetCast.Identity.Api.csproj", "Services/BudgetCast.Identity/src/BudgetCast.Identity.Api/"]
# sln
COPY ["Services/BudgetCast.Identity/BudgetCast.Identity.sln", "Services/BudgetCast.Identity/BudgetCast.Identity.sln"]

# restore (without arguments it uses .sln file by default - feature of .net core v3+)
# note: only runs if there's a !!!single!!! project or solution file in the folder
RUN dotnet restore "Services/BudgetCast.Identity/BudgetCast.Identity.sln"

# copy everything else
COPY ["Services/BudgetCast.Identity/src", "Services/BudgetCast.Identity/src"]

# build
RUN dotnet build "Services/BudgetCast.Identity/src/BudgetCast.Identity.Api/BudgetCast.Identity.Api.csproj" -c $CONFIGURATION -o /app/build

######################################## test stage #########################################

# no tests so far

######################################### publish app stage #########################################
FROM build AS publish
ARG CONFIGURATION
WORKDIR /src
RUN dotnet publish "Services/BudgetCast.Identity/src/BudgetCast.Identity.Api/BudgetCast.Identity.Api.csproj" -c $CONFIGURATION -o /app/publish --no-restore

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

ENTRYPOINT ["dotnet", "BudgetCast.Identity.Api.dll"]