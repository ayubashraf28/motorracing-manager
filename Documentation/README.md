# Project Documentation

This folder is the long-term source of truth for understanding, building, and operating `motorracing-manager`.

## How to Read This Documentation

1. Start with `Product/00-project-overview.md`.
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

## Maintenance Rule

Documentation is part of the product. Every functional or architectural change should update the relevant files in this folder in the same PR.
