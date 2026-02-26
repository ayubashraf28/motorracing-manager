# Runtime Bootstrap and Composition

## Current Startup Flow

1. Unity loads `Bootstrap.unity`.
2. `GameBootstrap` executes `Awake()`.
3. `Compose()` wires startup dependencies.
4. `LaunchInitialScreen()` asks App layer for initial screen and transitions when available.

## Composition Root Policy

- Runtime object graph creation starts in `MotorracingManager.Unity`.
- App-layer interfaces should be the default dependency boundary.
- Avoid service locator patterns spread across multiple scenes.

## Future Evolution Guidance

- Keep startup deterministic and idempotent.
- Introduce dedicated scene navigation abstraction before adding multiple scenes.
- Centralize feature registration in one composition entrypoint per platform.

## Anti-Patterns to Avoid

- Business rules in `MonoBehaviour.Update()` loops.
- Direct persistence calls from UI widgets.
- Cross-layer references that bypass `App` use cases.
