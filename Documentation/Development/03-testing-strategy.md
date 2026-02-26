# Testing Strategy

## Test Layers

- Unit tests (engine-free): primary coverage in package `Tests/Runtime`.
- Unity EditMode tests: validate package integration with Unity test runner.
- Build verification: CI ensures project can build for `StandaloneWindows64`.

## What to Prioritize

1. Deterministic simulation behavior.
2. App-layer use case behavior.
3. Persistence serialization/migration correctness.
4. Bootstrap and startup flow sanity.

## Test Placement Rules

- Tests for each package live inside that package under `Tests/Runtime`.
- Use `Tests/Editor` only when UnityEditor APIs are required.
- Keep tests close to the layer they validate.

## Minimum Expectation per Feature

- New business logic: add unit tests.
- New scene startup behavior: add at least one integration-style EditMode validation.
- Save schema change: add migration compatibility tests.

## CI Gate

PRs are expected to pass:

- Unity EditMode tests (`game-ci/unity-test-runner`).
- Windows build (`game-ci/unity-builder`).
