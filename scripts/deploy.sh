#!/usr/bin/env bash
set -euo pipefail

repository_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repository_root"

if [[ ! -f .env ]]; then
  echo ".env bulunamadı. Önce .env.example dosyasını .env olarak kopyalayıp değerleri doldurun."
  exit 1
fi

if [[ ! -f deploy/certs/fullchain.pem || ! -f deploy/certs/privkey.pem ]]; then
  echo "SSL sertifikaları deploy/certs altında bulunamadı."
  exit 1
fi

docker compose --env-file .env config >/dev/null
docker compose --env-file .env up --detach --build
docker compose --env-file .env ps
