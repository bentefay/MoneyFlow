#!/usr/bin/env bash

set -x
set -euo pipefail

ROOT=$(git rev-parse --show-toplevel)
DEV_DEPENDENCIES_PATH=${ROOT}/dev-dependencies

if [[ -z $* ]]
then
  ARGS=up
else
  ARGS=$*
fi

docker-compose -p moneyflow \
    -f ${DEV_DEPENDENCIES_PATH}/docker-compose.yml \
    ${ARGS}
