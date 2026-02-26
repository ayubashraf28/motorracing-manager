# System Overview

## Architectural Intent

The system is intentionally split into two zones:

- Engine-free domain/simulation/application logic.
- Engine-dependent adapters and UI.

This split protects core logic from Unity coupling and keeps tests fast.

## Runtime Entry Point

- Scene: `UnityProject/Assets/_Project/Scenes/Bootstrap.unity`
- Startup script: `MotorracingManager.Unity.GameBootstrap`

`GameBootstrap` is responsible for wiring startup services and selecting the initial screen.

## Layer Inventory

- `Core`: shared primitives and cross-cutting value types.
- `Domain`: entities, value objects, domain contracts.
- `Sim`: simulation behavior and deterministic business rules.
- `App`: use cases and orchestration.
- `Persistence`: save/load and state storage concerns.
- `Unity`: MonoBehaviour adapters and composition root.
- `UI`: presentation layer and screen controllers.

## Architectural Constraint

Business logic belongs in `Core/Domain/Sim/App`; Unity layers consume it through interfaces or use-case entry points.
