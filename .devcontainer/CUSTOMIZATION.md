# Customizing the .NET Devcontainer

This document provides guidance on how to customize the .NET devcontainer to fit your specific CLI tool development requirements.

## Modifying the .NET Version

If you need to use a different .NET version:

1. Update the `FROM` line in `.devcontainer/Dockerfile`:
   ```
   FROM mcr.microsoft.com/dotnet/sdk:X.Y
   ```
   Replace `X.Y` with your desired version (e.g., `8.0` for .NET 8)

2. Update `global.json` in your project:
   ```json
   {
     "sdk": {
       "version": "X.Y.ZZZ",
       "rollForward": "latestMajor"
     }
   }
   ```

## Adding Custom Tools and Extensions

### Adding VS Code Extensions

To add more VS Code extensions, modify the `extensions` array in `.devcontainer/devcontainer.json`:

```json
"extensions": [
    "ms-dotnettools.csharp",
    "your-new-extension-id"
]
```

### Installing Additional Tools in the Container

To install additional tools, modify the `Dockerfile`:

```Dockerfile
RUN apt-get update \
    && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends \
        your-package-name \
        another-package-name
```

### Adding Global .NET Tools

To add more global .NET tools:

```Dockerfile
RUN dotnet tool install -g tool-name
```

## Configuring Environment Variables

To add custom environment variables, modify the `devcontainer.json` file:

```json
"remoteEnv": {
    "MY_CUSTOM_VARIABLE": "value"
}
```

## Custom Post-Creation Commands

To run additional commands after the container is created:

```json
"postCreateCommand": "dotnet restore && your-additional-command"
```

## Debugging Configuration

To customize debugging settings, you can add a `.vscode` folder to your project with a `launch.json` file:

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (console)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/netX.Y/your-app.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "console": "internalConsole",
            "stopAtEntry": false
        }
    ]
}
```
