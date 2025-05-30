# StartFromRepo
A tool that can start a new GitHub project using another repo as a basis or template.

## Why?
I often create a repository that has a .devcontainer setup, scripts ready to go and other utilities that I want to re-use. I often clone it, copy the items over or manually set the new origin, etc. This tool does all that for you in one step. That may, you can maintain a few repos you like as a base to start working with, and get going with those really quickly.

**NOTE: WIP - NOT COMPLETE -**

## Usage

```bash
dotnet run --project src/StartFromRepo/StartFromRepo.csproj -- --username <github-username> --source <source-repo> --destination <destination-repo>
```

With push option:

```bash
dotnet run --project src/StartFromRepo/StartFromRepo.csproj -- --username <github-username> --source <source-repo> --destination <destination-repo> --push
```

Or using the short form:

```bash
dotnet run --project src/StartFromRepo/StartFromRepo.csproj -- -u <github-username> -s <source-repo> -d <destination-repo>
```

Short form with push option:

```bash
dotnet run --project src/StartFromRepo/StartFromRepo.csproj -- -u <github-username> -s <source-repo> -d <destination-repo> -p
```

### Parameters

- `--username` or `-u`: Your GitHub username
- `--source` or `-s`: Source repository name to copy from
- `--destination` or `-d`: Destination repository name to create
- `--push` or `-p`: Push code to GitHub repository after cloning - Github repo needs to be a manually created empty repo (optional, defaults to false)

### Authentication

The application utilizes your existing git credentials for GitHub authentication. Make sure you have configured git with your GitHub credentials using either:

1. SSH keys: `git config --global user.name "Your Name"` and `git config --global user.email "your.email@example.com"`
2. Git credential manager
3. GitHub CLI (`gh`) authentication

No additional authentication setup is required.

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
