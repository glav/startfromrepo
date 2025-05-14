# Use the latest .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set environment variables
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    DOTNET_NOLOGO=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=true

# Install additional tools
RUN apt-get update \
    && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends \
        git \
        curl \
        wget \
        nano \
        procps \
        zsh \
    && apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# Create a non-root user to use if preferred - see https://aka.ms/vscode-remote/containers/non-root-user.
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID

# Create the user
RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
    # Add sudo support
    && apt-get update \
    && apt-get install -y sudo \
    && echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME \
    && chmod 0440 /etc/sudoers.d/$USERNAME

# Set zsh as the default shell for the user
RUN chsh -s /bin/zsh $USERNAME

# Set the default user
USER $USERNAME

# Configure global .NET CLI tools path
ENV PATH="${PATH}:/home/${USERNAME}/.dotnet/tools"
RUN mkdir -p /home/${USERNAME}/.dotnet/tools

# Install global .NET tools
RUN dotnet tool install -g dotnet-format \
    && dotnet tool install -g dotnet-ef \
    && dotnet tool install -g dotnet-trace \
    && dotnet tool install -g dotnet-counters

# Set the default working directory
WORKDIR /workspace
