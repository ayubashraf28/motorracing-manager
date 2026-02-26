# Project Overview

## Purpose

`motorracing-manager` is a Unity-based motorsport management game codebase organized around strict architectural layers.

## Current Scope

- Unity monorepo with local package modularization.
- Engine-free business layers for deterministic, testable logic.
- Unity adapter and UI layers for engine integration.
- CI pipeline for EditMode tests and Windows build validation.

## Non-Goals (Current Phase)

- Full gameplay systems implementation.
- Complete UI/UX feature set.
- Production release pipeline beyond build artifacts.

## Repository Top-Level Structure

- `.github/workflows/`: GitHub Actions automation.
- `Documentation/`: this documentation system.
- `UnityProject/`: Unity project source and settings.

## Definitions

- `Engine-free`: assembly has `noEngineReferences: true`, cannot use `UnityEngine` APIs.
- `Adapter`: Unity-facing component that translates engine events into app-layer actions.
- `Composition root`: startup location where runtime dependencies are wired.
