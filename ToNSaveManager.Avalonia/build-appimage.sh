#!/bin/sh
# Builds artifacts/ToNSaveManager-x86_64.AppImage from the committed sources.
#
# Usage: build-appimage.sh [--version X] [--index FILE] [--tmp DIR]
#   --version  App version, defaults to the date like the release workflow (yyyy.M.d.minutes)
#   --index    Use this index.json instead of the one in git
#   --tmp      Where to build, defaults to $TMPDIR or /tmp (needs about 400 MB)
set -eu

VERSION="$(date +%Y.%-m.%-d).$(( $(date +%-H) * 60 + $(date +%-M) ))"
INDEX=""
TMP="${TMPDIR:-/tmp}"

while [ $# -gt 0 ]; do
    case "$1" in
        --version) VERSION="$2"; shift 2 ;;
        --index) INDEX="$(readlink -f "$2")"; shift 2 ;;
        --tmp) TMP="$2"; shift 2 ;;
        *) echo "Unknown option: $1" >&2; exit 1 ;;
    esac
done

if ! command -v dotnet >/dev/null; then
    echo "dotnet not found, install the .NET 8 SDK: https://dotnet.microsoft.com/download" >&2
    exit 1
fi
if ! command -v appimagetool >/dev/null; then
    echo "appimagetool not found, get it from https://github.com/AppImage/appimagetool/releases" >&2
    exit 1
fi
if [ -n "$INDEX" ] && [ ! -f "$INDEX" ]; then
    echo "Index not found: $INDEX" >&2
    exit 1
fi

ROOT="$(git -C "$(dirname "$0")" rev-parse --show-toplevel)"
if [ -n "$(git -C "$ROOT" status --porcelain --untracked-files=no)" ]; then
    echo "Note: uncommitted changes are not included"
fi

WORK="$(mktemp -d "$TMP/tonsm-appimage.XXXXXX")"
trap 'rm -rf "$WORK"' EXIT

SRC="$WORK/src"
APPDIR="$WORK/ToNSaveManager.AppDir"
mkdir -p "$SRC" "$APPDIR/usr/bin"

git -C "$ROOT" archive HEAD | tar -x -C "$SRC"
if [ -n "$INDEX" ]; then
    echo "Using index: $INDEX"
    cp "$INDEX" "$SRC/Resources/index.json"
fi

echo "Building version $VERSION"
dotnet publish "$SRC/ToNSaveManager.Avalonia/ToNSaveManager.Avalonia.csproj" \
    -c Release -r linux-x64 --self-contained true \
    -p:Version="$VERSION" -o "$APPDIR/usr/bin" -nologo -v q

cat > "$APPDIR/AppRun" <<'EOF'
#!/bin/sh
HERE="$(dirname "$(readlink -f "$0")")"
exec "$HERE/usr/bin/ToNSaveManager" "$@"
EOF
chmod +x "$APPDIR/AppRun"

cat > "$APPDIR/tonsavemanager.desktop" <<'EOF'
[Desktop Entry]
Type=Application
Name=ToNSaveManager
Comment=Save manager for Terrors of Nowhere
Exec=ToNSaveManager
Icon=tonsavemanager
Categories=Game;Utility;
Terminal=false
EOF

cp "$SRC/Resources/xs_icon.png" "$APPDIR/tonsavemanager.png"
ln -s tonsavemanager.png "$APPDIR/.DirIcon"

mkdir -p "$ROOT/artifacts"
OUT="$ROOT/artifacts/ToNSaveManager-x86_64.AppImage"
ARCH=x86_64 appimagetool --no-appstream "$APPDIR" "$OUT"
echo "Done: $OUT"
