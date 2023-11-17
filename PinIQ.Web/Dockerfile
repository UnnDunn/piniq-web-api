# syntax=docker/dockerfile:1

# Comments are provided throughout this file to help you get started.
# If you need more help, visit the Dockerfile reference guide at
# https://docs.docker.com/engine/reference/builder/

################################################################################

# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md

# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source

RUN curl -L https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh  | sh
ARG FEED_ACCESSTOKEN

# Restore as a distinct layer
COPY Pinball.Api.Data/*.csproj Pinball.Api.Data/
COPY Pinball.Api.Services/*.csproj Pinball.Api.Services/
COPY Pinball.Opdb.Client/*.csproj Pinball.Opdb.Client/
COPY Pinball.Api/*.csproj Pinball.Api/
COPY NuGet.config NuGet.config

ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/unndunn/_packaging/UnnDunn-Nuget/nuget/v3/index.json\", \"username\":\"unndunn@hotmail.com\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore "Pinball.Api/Pinball.Api.csproj"

# Build the application.
# Leverage a cache mount to /root/.nuget/packages so that subsequent builds don't have to re-download packages.
COPY Pinball.Api.Data/ Pinball.Api.Data/
COPY Pinball.Api.Services/ Pinball.Api.Services/
COPY Pinball.Opdb.Client/ Pinball.Opdb.Client/
COPY Pinball.Api/ Pinball.Api/


FROM build AS publish
WORKDIR /source/Pinball.Api
RUN  --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish --no-restore -o /app
COPY *.p8 /app

# If you need to enable globalization and time zones:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md
################################################################################
# Create a new stage for running the application that contains the minimal
# runtime dependencies for the application. This often uses a different base
# image from the build stage where the necessary files are copied from the build
# stage.
#
# The example below uses an aspnet alpine image as the foundation for running the app.
# It will also use whatever happens to be the most recent version of that tag when you
# build your Dockerfile. If reproducability is important, consider using a more specific
# version (e.g., aspnet:7.0.10-alpine-3.18),
# or SHA (e.g., mcr.microsoft.com/dotnet/aspnet@sha256:f3d99f54d504a21d38e4cc2f13ff47d67235efeeb85c109d3d1ff1808b38d034).
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Add ICU support to alpine image, required for SqlClient
ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

RUN apk add --no-cache \
    icu-data-full \
    icu-libs

# Copy everything needed to run the app from the "build" stage.
COPY --from=publish /app .

# Create a non-privileged user that the app will run under.
# See https://docs.docker.com/develop/develop-images/dockerfile_best-practices/#user
ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --shell "/sbin/nologin" \
    --uid "${UID}" \
    appuser
USER appuser

ENTRYPOINT ["dotnet", "Pinball.Api.dll"]
