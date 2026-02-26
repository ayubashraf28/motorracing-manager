# CI/CD

## Workflow

- File: `.github/workflows/unity-ci.yml`
- Trigger: `pull_request` and `push` to `main`.
- Runner: `windows-latest`.
- Repository visibility: public.

## Pipeline Steps

1. Checkout repository with LFS support.
2. Cache `UnityProject/Library` keyed by manifest and project version.
3. Run EditMode tests with GameCI.
4. Build Windows player (`StandaloneWindows64`).
5. Upload build artifacts.

## Required Repository Secrets

- `UNITY_LICENSE` (configured).

## Branch Protection Baseline

- `main` is protected and merge-only via pull requests.
- Required status check: temporarily disabled (as of February 26, 2026) due Unity activation blocker in CI.
- Admin enforcement: enabled.
- Force-push: disabled.
- Branch deletion: disabled.
- Conversation resolution before merge: enabled.

## Temporary CI Exception

- Unity CI still runs on PR/push and remains advisory while activation is being corrected.
- Condition to restore strict gating: a validated GameCI activation strategy and at least one successful `test-and-build-windows` run on an active PR branch.
- Once validated, `test-and-build-windows` must be re-added as a required status check on `main`.

## Operational Notes

- Keep manifest and project version deterministic to maximize cache reuse.
- If pipeline fails after package changes, verify package dependency and asmdef references first.
- CI is treated as a quality gate, not a post-merge notification.
- If workflow check names change, update branch protection and this file together.
