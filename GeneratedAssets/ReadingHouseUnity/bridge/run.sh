#!/bin/bash
# usage: run.sh <command> [refresh-first]
cd "$(dirname "$0")"
LOG="$LOCALAPPDATA/Unity/Editor/Editor.log"
if [ "$2" = "refresh" ]; then
  START=$(stat -c %s "$LOG"); OLD=$(cat ready.txt); echo refresh > command.txt
  for i in $(seq 1 200); do [ "$(cat ready.txt)" != "$OLD" ] && break
    tail -c +$START "$LOG" | grep -q "error CS" && { tail -c +$START "$LOG" | grep "error CS" | sort -u | head; exit 1; }; sleep 2; done
  sleep 3
fi
rm -f result.txt log.txt; echo "$1" > command.txt
for i in $(seq 1 900); do [ -f result.txt ] && break; sleep 2; done
cat result.txt; echo
grep -E "^\[(Error|Exception)\]" log.txt | sort | uniq -c | head -10
