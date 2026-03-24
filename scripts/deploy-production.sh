#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${DEPLOY_WEBHOOK_URL:-}" || -z "${DEPLOY_TOKEN:-}" ]]; then
  echo "🚧 Deploy not configured yet. Skipping production deploy."
  echo "Commit: ${GITHUB_SHA:-local}"
  echo "Branch: ${GITHUB_REF_NAME:-unknown}"
  exit 0
fi

echo "Triggering production deployment..."
curl -fsS -X POST "$DEPLOY_WEBHOOK_URL" \
  -H "Authorization: Bearer $DEPLOY_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"service\":\"try-me-tumble-api\",
    \"environment\":\"production\",
    \"commit\":\"${GITHUB_SHA:-local}\"
  }"

echo "Deploy trigger sent successfully."