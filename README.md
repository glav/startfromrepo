# StartFromRepo
A tool that can start a new GitHub project using another repo as a basis or template.

## Usage

```bash
dotnet run --project src/StartFromRepo/StartFromRepo.csproj -- --username <github-username> --source <source-repo> --destination <destination-repo>
```

Or using the short form:

```bash
dotnet run --project src/StartFromRepo/StartFromRepo.csproj -- -u <github-username> -s <source-repo> -d <destination-repo>
```

### Parameters

- `--username` or `-u`: Your GitHub username
- `--source` or `-s`: Source repository name to copy from
- `--destination` or `-d`: Destination repository name to create

### Authentication

The app uses interactive authentication with a GitHub Personal Access Token. When prompted, enter your GitHub token to authenticate.

## .NET C# Development Environment

This repository includes a devcontainer configuration for .NET C# development. The devcontainer is configured with the latest .NET 9 SDK and all the necessary tools for CLI tool development.

### Features

- Latest .NET 9 SDK
- VS Code extensions for C# development
- Global .NET tools pre-installed
- Sample CLI project structure

### Getting Started

1. Open this repository in VS Code
2. When prompted, click "Reopen in Container"
3. The devcontainer will build and provide a fully configured .NET development environment

For more detailed instructions, see [.devcontainer/README.md](.devcontainer/README.md).
