#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${DEPLOY_WEBHOOK_URL_STAGING:-}" || -z "${DEPLOY_TOKEN:-}" ]]; then
  echo "🚧 Staging deploy not configured yet. Skipping."
  echo "Commit: ${GITHUB_SHA:-local}"
  echo "Branch: ${GITHUB_REF_NAME:-unknown}"
  exit 0
fi

echo "Triggering staging deployment..."
curl -fsS -X POST "$DEPLOY_WEBHOOK_URL_STAGING" \
  -H "Authorization: Bearer $DEPLOY_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"service\":\"try-me-tumble-api\",
    \"environment\":\"staging\",
    \"commit\":\"${GITHUB_SHA:-local}\"
  }"

echo "Staging deploy trigger sent successfully."