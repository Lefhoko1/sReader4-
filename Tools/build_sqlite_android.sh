#!/usr/bin/env bash
# Builds libe_sqlite3.so from the SQLite amalgamation for every Android ABI,
# using Unity's bundled NDK, and drops each into the matching Unity plugin folder.
# These are loaded by SQLitePCLRaw's provider.e_sqlite3 (DllImport "e_sqlite3"),
# which only needs the standard sqlite3_* symbols — built with default visibility
# here so they are exported (fixes the "symbols hidden" load failure on 32-bit).
#
# Usage:  bash Tools/build_sqlite_android.sh /path/to/sqlite3.c
set -euo pipefail

SRC="${1:?Pass the path to sqlite3.c (from the sqlite-amalgamation zip)}"
NDK="/c/Program Files/Unity/Hub/Editor/6000.4.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/NDK"
BIN="$NDK/toolchains/llvm/prebuilt/windows-x86_64/bin"
PLUGINS="$(cd "$(dirname "$0")/.." && pwd)/Assets/Plugins/Android/libs"
API=25

# Standard SQLite build flags (mirrors the common e_sqlite3 feature set).
FLAGS="-shared -fPIC -O2 -fvisibility=default \
  -DSQLITE_ENABLE_FTS5 -DSQLITE_ENABLE_RTREE -DSQLITE_ENABLE_JSON1 \
  -DSQLITE_ENABLE_COLUMN_METADATA -DSQLITE_ENABLE_DBSTAT_VTAB \
  -DSQLITE_DEFAULT_FOREIGN_KEYS=1 -DSQLITE_THREADSAFE=1"

# ABI -> clang target triple
build() {
  local abi="$1" cc="$2"
  echo ">> $abi"
  mkdir -p "$PLUGINS/$abi"
  "$BIN/$cc$API-clang" $FLAGS -o "$PLUGINS/$abi/libe_sqlite3.so" "$SRC" -lm
  echo "   $(ls -la "$PLUGINS/$abi/libe_sqlite3.so" | awk '{print $5, "bytes"}')"
}

build armeabi-v7a armv7a-linux-androideabi
build arm64-v8a   aarch64-linux-android
build x86         i686-linux-android
build x86_64      x86_64-linux-android

echo "Done. Rebuild the APK."
