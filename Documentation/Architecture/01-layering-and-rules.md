# Layering and Dependency Rules

## Allowed Dependency Direction

`Core -> Domain -> Sim -> App -> Persistence -> Unity -> UI`

Additional allowed references from current package setup:

- `UI` may reference `Domain`, `App`, and `Unity`.
- `Unity` references `App` and `Persistence`.

## Hard Rules

1. `Core`, `Domain`, `Sim`, `App`, `Persistence` must remain engine-free (`noEngineReferences: true`).
2. `UnityEngine` references are allowed only in `Unity` and `UI` packages.
3. `MonoBehaviour` classes must not contain business rules.
4. Domain state is persisted, not scene object state.

## Why This Matters

- Faster, deterministic tests.
- Lower refactor risk.
- Cleaner separation between gameplay logic and rendering/input lifecycle.

## Review Checklist for Dependency Safety

- Does any engine-free package reference `UnityEngine`?
- Is new logic placed in `App/Sim/Domain` instead of `MonoBehaviour`?
- Does a new package preserve directional dependencies?
- Are asmdef references minimal and explicit?
