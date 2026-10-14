#!/usr/bin/env bash
set -euo pipefail

dotnet restore EsgEnergy.sln
dotnet build EsgEnergy.sln --configuration Release --no-restore
dotnet test EsgEnergy.sln --configuration Release --no-build

if command -v docker >/dev/null 2>&1; then
  docker compose --env-file .env.example config --quiet
  echo "Configuração Docker Compose válida."
else
  echo "Docker não encontrado; etapa de Compose ignorada."
fi
