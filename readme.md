# PinIQ Web API

## Welcome

This is the API that will serve all PinIQ-related clients going forward.

## Dependencies

### 1. DotNet SDK

#### Windows:

Installed with Visual Studio, or use winget to install it from an elevated command line:

    winget install Microsoft.DotNet.SDK

#### Mac:

Installed with Visual Studio for Mac, or install it with [Homebrew](https://brew.sh):

    brew install --cask dotnet-sdk

### 2. Docker Desktop

#### Windows:

Install with winget from an elevated command-line:

    winget install Docker.Desktop

#### Mac:

Install with Homebrew:

    brew install --cask docker

## Setup

### 1. Create `.env` file

Create an `.env` file at the root level of the project (where the `compose.yaml` file is) with the following contents:

    UNNDUNN_FEED_ACCESSTOKEN=
    DOTNET_USERSECRETS_PATH=${APPDATA}/Microsoft/UserSecrets
    CERTIFICATE_PATH=${USERPROFILE}/.aspnet/https

Contact [Uchendu](mailto:unndunn@hotmail.com) to receive an access token for the UnnDunn-NuGet feed.

`DOTNET_USERSECRETS_PATH` should be as above on Windows, or `~/.microsoft/UserSecrets` on Mac.

`CERTIFICATE_PATH` should be as above on Windows, or `~/.aspnet/https` on Mac.

### 2. Generate .Net dev certificates

Open a PowerShell prompt or terminal in the `Pinball.Api/` directory.

Run `dotnet dev-certs https -v --clean` to remove any existing developer certificates.

On Windows (using PowerShell), run:

    dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\Pinball.Api.pfx -p password
    dotnet dev-certs https --trust

On Mac using bash, run:

    dotnet dev-certs https -ep ~/.aspnet/https/Pinball.Api.pfx -p password
    dotnet dev-certs https --trust

## Build and Run

Build and run the project using Docker Compose:

    docker compose --build up d

API endpoints will be accessible from `https://localhost:8001` or `http://localhost:8000`.