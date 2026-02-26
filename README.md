# motorracing-manager

Layered Unity monorepo for a motorsport management game.

## Repository Layout

- `.github/workflows/` CI definitions
- `Documentation/ADR/` architecture decisions
- `UnityProject/` Unity project root
- `UnityProject/Assets/_Project/` project-owned scenes/settings/resources
- `UnityProject/Packages/com.alivastudio.motorracingmanager.*` local packages

## Unity Version

- Unity `6000.3.x` (project currently seeded with `6000.3.10f1`)

## Local Setup

```powershell
git lfs install
git lfs track "*.psd" "*.png" "*.tga" "*.exr" "*.wav" "*.mp3" "*.ogg" "*.fbx" "*.blend" "*.mp4" "*.mov"
```

Open `UnityProject/` in Unity Hub using Unity 6000.3.x.

## CI / License Prerequisite

GitHub Actions workflow requires repository secret:

- `UNITY_LICENSE`

## Package Dependency Graph

`Core -> Domain -> Sim -> App -> Persistence -> Unity -> UI`

## Composition Root Rule

- `MonoBehaviour` classes are adapters only.
- Business rules stay in `Core/Domain/Sim/App`.
- `GameBootstrap` is the scene entrypoint and delegates to App-layer use cases.

## Documentation

- Start at `Documentation/README.md`.
- Architecture: `Documentation/Architecture/`
- Developer onboarding/workflow/testing: `Documentation/Development/`
- CI and operational guidance: `Documentation/Operations/`
- Governance and update policy: `Documentation/Governance/`
- Decision records: `Documentation/ADR/`

Documentation is maintained continuously and should be updated in the same PR as related code changes.

## Branch Workflow

- `main` is protected.
- Create `feature/*` branches for all work.
- Merge via pull request after required check `test-and-build-windows` passes.
