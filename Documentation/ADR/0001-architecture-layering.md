# ADR 0001: Layered architecture with engine-free simulation

## Decision
We separate the game into engine-free layers (Core/Domain/Sim/App/Persistence) and engine-dependent layers (Unity/UI).
Domain and Simulation assemblies must not reference UnityEngine.

## Rationale
- Enables fast unit tests and deterministic simulation debugging
- Prevents UI/MonoBehaviours from becoming the business logic layer
- Supports long-term maintainability and refactoring

## Consequences
- All Unity-facing code must be adapters that call into App use cases
- Save/load persists domain state, not scene state
