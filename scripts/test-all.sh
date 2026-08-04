#!/usr/bin/env bash
set -euo pipefail

repository_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

dotnet test "$repository_root/TaskManagement.API.Tests/TaskManagement.API.Tests.csproj" \
  --configuration Release \
  --disable-build-servers

cd "$repository_root/TaskManagement.Web"
npm ci
CI=1 npm test -- --watch=false
CI=1 npm run build -- --configuration production --progress=false
