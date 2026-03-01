# Project Documentation

This folder is the long-term source of truth for understanding, building, and operating `motorracing-manager`.

## How to Read This Documentation

1. Start with `Product/00-project-overview.md`.
1b. Read `Product/01-game-pillars.md` for non-negotiable design rules.
1c. Read `Product/02-performance-philosophy.md` for the lap time model.
1d. Read `Product/03-core-loop-specification.md` for the game loop and tick order.
1e. Read `Product/04-screen-flow-map.md` for screen navigation and UI structure.
1f. Read `Product/05-definition-types.md` for immutable definition pack architecture.
2. Read `Architecture/00-system-overview.md` and `Architecture/01-layering-and-rules.md`.
3. Review `Development/00-getting-started.md` for local setup.
4. Use `Operations/00-ci-cd.md` for pipeline and deployment behavior.
5. Consult `ADR/` for architectural decisions and rationale history.

## Documentation Map

- `ADR/`: architecture decisions and trade-offs over time.
- `Architecture/`: system structure, package boundaries, runtime flow, and persistence model.
- `Development/`: coding standards, workflow, setup, and testing guidance.
- `Operations/`: CI/CD, release process, and operational readiness.
- `Governance/`: rules for maintaining and evolving documentation quality.
- `Templates/`: reusable templates for ADRs and feature specs.
- `Product/`: project goals, scope, and shared terminology.

## Repository Workflow Baseline

- `main` is protected and is merge-only through pull requests.
- Required status check on `main`: temporarily optional while Unity activation is remediated.
- Repository visibility is currently public.
- Required check to restore after remediation: `test-and-build-windows`.

## Maintenance Rule

Documentation is part of the product. Every functional or architectural change should update the relevant files in this folder in the same PR.
