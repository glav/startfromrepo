# .NET C# Development Container

This development container provides a fully configured environment for .NET C# CLI tool development.

## Features

- Latest .NET 9 SDK
- C# development extensions for VS Code
- Git integration
- Global .NET tools pre-installed:
  - dotnet-format: For code formatting
  - dotnet-ef: Entity Framework Core CLI tools
  - dotnet-trace: .NET tracing tool
  - dotnet-counters: Performance counters monitoring

## Getting Started

1. Open this repository in VS Code
2. When prompted, click "Reopen in Container"
3. Wait for the container to build (this may take a few minutes the first time)
4. After the container is built, you'll have a fully configured .NET development environment

## Creating a New C# CLI Project

To create a new C# CLI project, use the following commands in the terminal:

```bash
# Create a new console application
dotnet new console -n MyCliTool

# Navigate to the project directory
cd MyCliTool

# Run the application
dotnet run
```

## Adding Dependencies

To add NuGet packages to your project:

```bash
dotnet add package PACKAGE_NAME
```

## Building and Publishing

To build your CLI tool:

```bash
dotnet build
```

To publish a self-contained executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained true
dotnet publish -c Release -r linux-x64 --self-contained true
dotnet publish -c Release -r osx-x64 --self-contained true
```

## Included Tools and Extensions

- C# language support
- .NET Test Explorer
- NuGet Package Manager
- EditorConfig support
- C# Extensions (class generators, etc.)
- Code Spell Checker
- PowerShell support
