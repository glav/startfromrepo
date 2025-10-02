#!/bin/bash

echo Setting executable permissions for publish scripts...

chmod +x ./src/scripts/publish-windows-x64-binary.sh
chmod +x ./src/scripts/publish-macos-intel-binary.sh
chmod +x ./src/scripts/publish-macos-arm-binary.sh
chmod +x ./src/scripts/publish-linux-x64-binary.sh

echo ----------------------------------------------
echo If you want help using the prompt library or list
echo of available prompts, simply type:
echo 'What prompts are available to use?'
echo ----------------------------------------------
