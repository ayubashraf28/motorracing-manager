# Project Overview

## Purpose

`motorracing-manager` is a Unity-based motorsport management game codebase organized around strict architectural layers.

## Current Scope

- Unity monorepo with local package modularization.
- Engine-free business layers for deterministic, testable logic.
- Unity adapter and UI layers for engine integration.
- CI pipeline for EditMode tests and Windows build validation.
- Game design pillars locked (see Product/01-game-pillars.md).
- Lap time performance philosophy locked (see Product/02-performance-philosophy.md).
- Core game loop specified (see Product/03-core-loop-specification.md).
- Screen flow map locked (see Product/04-screen-flow-map.md).
- Immutable definition-type architecture locked (see Product/05-definition-types.md).

## Non-Goals (Current Phase)

- Full gameplay systems implementation.
- Complete UI/UX feature set.
- Production release pipeline beyond build artifacts.

## Repository Top-Level Structure

- `.github/workflows/`: GitHub Actions automation.
- `Documentation/`: this documentation system.
- `UnityProject/`: Unity project source and settings.

## Collaboration Model

- Repository visibility is public.
- `main` is a protected branch.
- Code changes are expected to land through feature branches and pull requests.

## Definitions

- `Engine-free`: assembly has `noEngineReferences: true`, cannot use `UnityEngine` APIs.
- `Adapter`: Unity-facing component that translates engine events into app-layer actions.
- `Composition root`: startup location where runtime dependencies are wired.
