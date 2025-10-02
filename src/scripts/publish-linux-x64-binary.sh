#!/usr/bin/bash

dotnet publish src/startfromrepo/startfromrepo.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true
