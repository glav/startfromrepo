# StartFromRepo

A command-line tool that creates a new GitHub project using another repository as a template, automatically handling cloning, remote configuration, and optional pushing to a new repository.

## Overview

StartFromRepo streamlines the process of starting new projects from existing repository templates. Instead of manually cloning, changing remotes, and pushing to new repositories, this tool handles the entire workflow in a single command.

## Why?

When starting new projects, developers often want to reuse existing repository structures that include:
- Preconfigured `.devcontainer` setups
- Build scripts and CI/CD workflows
- Project scaffolding and utilities
- Development tooling configuration

This tool automates the tedious process of cloning a template repository and setting it up with a new remote origin, optionally pushing the code to a new GitHub repository in one step.

## Features

- ✅ Clone any GitHub repository as a template
- ✅ Automatically reconfigure git remote to point to new repository
- ✅ Rename default branch to `main`
- ✅ Optional automatic push to GitHub (requires pre-created empty repository)
- ✅ Uses existing git credentials (no additional authentication required)
- ✅ Comprehensive logging with color-coded output
- ✅ Cross-platform support (Linux, macOS, Windows)

## Quick Start

### Using with .NET CLI

```bash
dotnet run --project src/startfromrepo/startfromrepo.csproj -- --username <github-username> --source <source-repo> --destination <destination-repo>
```

### Using a Published Binary

```bash
./StartFromRepo --username <github-username> --source <source-repo> --destination <destination-repo>
```

## Usage

### Basic Usage

Clone a repository and set up new remote:

```bash
dotnet run --project src/startfromrepo/startfromrepo.csproj -- \
  --username myusername \
  --source template-repo \
  --destination my-new-project
```

### With Auto-Push

Clone and automatically push to a pre-created GitHub repository:

```bash
dotnet run --project src/startfromrepo/startfromrepo.csproj -- \
  --username myusername \
  --source template-repo \
  --destination my-new-project \
  --push
```

### Short Form

```bash
dotnet run --project src/startfromrepo/startfromrepo.csproj -- \
  -u myusername \
  -s template-repo \
  -d my-new-project \
  -p
```

### Command-Line Parameters

| Parameter | Short | Description | Required | Default |
|-----------|-------|-------------|----------|---------|
| `--username` | `-u` | GitHub username | Yes | - |
| `--source` | `-s` | Source repository name to clone | Yes | - |
| `--destination` | `-d` | Destination repository name | Yes | - |
| `--push` | `-p` | Push to GitHub after cloning | No | `false` |

### Authentication

The tool uses your existing git credentials configured on your system. Ensure you have one of the following configured:

1. **Git Credential Manager** (recommended for HTTPS)
2. **SSH Keys** configured with GitHub
3. **GitHub CLI (`gh`)** authentication

Verify your git configuration:
```bash
git config --get user.name
git config --get user.email
```

### Push Flag Behavior

When using the `--push` flag:
1. The tool checks if the destination repository exists on GitHub
2. If it exists, the code is automatically pushed to `origin/main`
3. If it doesn't exist, you'll receive instructions to create the repository manually

**Important:** Create an empty GitHub repository at `https://github.com/<username>/<destination>` before using `--push`.

## Architecture

### Key Components

#### `Program.cs`
Main entry point that handles:
- Command-line argument parsing using `System.CommandLine`
- Orchestration of the cloning and pushing workflow
- User feedback through the logging utility

#### `GitCliUtility.cs`
Git operations wrapper providing:
- `VerifyGitCredentialsAsync()` - Validates git configuration
- `CloneRepositoryAsync()` - Clones source repository
- `ChangeGitOriginAsync()` - Updates remote origin and ensures `main` branch
- `CheckRepositoryExistsAsync()` - Verifies GitHub repository existence
- `PushCodeToRepositoryAsync()` - Pushes code to remote repository
- `ExecuteGitCommandAsync()` - Low-level git command execution

#### `LoggingUtility.cs`
Console logging utility with color-coded output:
- `LogInfo()` - White text for informational messages
- `LogError()` - Red text for error messages
- `LogDebug()` - Yellow text for debug information

### Workflow

```
User Input → Parse Arguments → Verify Git Credentials
    ↓
Clone Source Repository → Change Remote Origin → Rename to 'main'
    ↓
(Optional) Check Repository Exists → Push to GitHub
    ↓
Success/Error Feedback
```

## Development

### Prerequisites

- .NET 9.0 SDK
- Git
- GitHub account with configured credentials

### Building the Project

```bash
# Restore dependencies
dotnet restore src/startfromrepo.sln

# Build in Debug mode
dotnet build src/startfromrepo.sln

# Build in Release mode
dotnet build src/startfromrepo.sln --configuration Release
```

### Running Tests

The project includes xUnit tests for core functionality:

```bash
# Run all tests
dotnet test src/Tests/startfromrepo.Tests.csproj

# Run with verbose output
dotnet test src/Tests/startfromrepo.Tests.csproj --verbosity normal
```

Test files:
- `GitTests.cs` - Tests for Git utility functions
- `ProgramTests.cs` - Tests for main program logic
- `LoggingTests.cs` - Tests for logging utility

### Publishing Standalone Binaries

Create self-contained executables that don't require .NET runtime installation:

#### Linux (x64)
```bash
dotnet publish src/startfromrepo/startfromrepo.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true
```

#### Windows (x64)
```bash
dotnet publish src/startfromrepo/startfromrepo.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true
```

#### macOS (ARM - M1/M2/M3)
```bash
dotnet publish src/startfromrepo/startfromrepo.csproj \
  -c Release \
  -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true
```

#### macOS (Intel)
```bash
dotnet publish src/startfromrepo/startfromrepo.csproj \
  -c Release \
  -r osx-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true
```

Published binaries will be located in: `src/startfromrepo/bin/Release/net9.0/<runtime-id>/publish/`

## CI/CD

The project uses GitHub Actions for continuous integration:

- **Workflow**: `.github/workflows/ci.yml`
- **Triggers**: Pull requests and pushes to `main` branch
- **Jobs**:
  - Restore NuGet packages with caching
  - Build solution in Release mode
  - Run xUnit test suite

## Project Structure

```
startfromrepo/
├── src/
│   ├── startfromrepo/
│   │   ├── Program.cs              # Main entry point
│   │   ├── GitCliUtility.cs        # Git operations wrapper
│   │   ├── LoggingUtility.cs       # Console logging utility
│   │   ├── startfromrepo.csproj    # Project file
│   │   └── global.json             # .NET SDK version
│   ├── Tests/
│   │   ├── GitTests.cs             # Git utility tests
│   │   ├── ProgramTests.cs         # Program logic tests
│   │   ├── LoggingTests.cs         # Logging tests
│   │   └── startfromrepo.Tests.csproj
│   └── startfromrepo.sln           # Solution file
├── .github/
│   ├── workflows/
│   │   └── ci.yml                  # CI/CD workflow
│   └── prompts/                    # Custom Copilot prompts
├── .devcontainer/                  # Dev container configuration
└── README.md
```

## Dependencies

- **System.CommandLine** (2.0.0-beta) - Modern command-line parsing
- **xUnit** (2.6.6) - Unit testing framework
- **.NET 9.0** - Target framework

## .NET Development Environment

This repository includes a preconfigured devcontainer for .NET C# development with:

- .NET 9.0 SDK
- VS Code C# extensions
- Git and GitHub CLI tools
- Pre-installed global .NET tools

### Getting Started with DevContainer

1. Open this repository in VS Code
2. When prompted, click "Reopen in Container"
3. The devcontainer will build with a fully configured development environment

For detailed instructions, see [.devcontainer/README.md](.devcontainer/README.md).

## License

See [LICENSE](LICENSE) for details.

## Contributing

Contributions are welcome! Please ensure:
- All tests pass: `dotnet test`
- Code builds without warnings: `dotnet build`
- Follow existing code style and patterns

## Version

Current version: **0.4.0**
