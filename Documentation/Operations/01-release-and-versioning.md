# Release and Versioning

## Current Stage

Project is in early foundation stage (`0.x` package versions).

## Versioning Approach

- Local package versions use semantic intent:
  - Patch: backward-compatible fix.
  - Minor: backward-compatible feature.
  - Major: breaking API/contract change.

## Release Readiness Checklist

- CI passing on target branch.
- Critical docs updated (architecture/testing/operations).
- ADR added for significant architecture trade-off.
- Save-data compatibility reviewed if persistence changed.

## Suggested Near-Term Evolution

- Add changelog policy once first feature milestone lands.
- Add artifact naming/version stamping strategy in CI.
- Add branch protection rules aligned with CI checks.
