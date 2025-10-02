#!/bin/bash

dotnet publish src/startfromrepo/startfromrepo.csproj \
  -c Release \
  -r osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true
