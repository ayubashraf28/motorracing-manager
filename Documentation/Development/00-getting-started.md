# Getting Started

## Prerequisites

- Windows machine with Unity Hub.
- Unity Editor `6000.3.10f1` (or compatible `6000.3.x`).
- Git + Git LFS.
- GitHub CLI (`gh`) for repo operations.

## Initial Setup

```powershell
git lfs install
git lfs track "*.psd" "*.png" "*.tga" "*.exr" "*.wav" "*.mp3" "*.ogg" "*.fbx" "*.blend" "*.mp4" "*.mov"
```

## Open Project

1. Open Unity Hub.
2. Add/open `UnityProject/` folder.
3. Use Unity `6000.3.10f1`.
4. Let packages/import complete.

## Required Editor Settings

- Version Control: `Visible Meta Files`
- Asset Serialization: `Force Text`
- Active Input Handling: `Input System Package`

## First Validation

- Confirm `Bootstrap.unity` opens without missing script warnings.
- Confirm package assemblies compile.
- Confirm EditMode tests appear in Test Runner.
