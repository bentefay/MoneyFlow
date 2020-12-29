#!/usr/bin/env bash

set -x
set -euo pipefail

ROOT=$(git rev-parse --show-toplevel)
DEV_DEPENDENCIES_PATH=${ROOT}/dev-dependencies
RUN="docker-compose -p moneyflow -f ${DEV_DEPENDENCIES_PATH}/docker-compose.yml"

if [[ -z $* ]]
then
  ARGS=up
else
  ARGS=$*
fi

$RUN down
$RUN ${ARGS}
