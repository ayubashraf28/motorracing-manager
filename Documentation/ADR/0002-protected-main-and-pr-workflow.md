# ADR 0002: Protected `main` and Pull-Request Workflow

## Status
Accepted

## Context

As the project grows, direct pushes to `main` create avoidable risk:

- accidental regressions,
- unreviewed structural changes,
- reduced traceability for why changes were made.

The repository also needs consistent enforcement of CI quality gates before changes land.

## Decision

We enforce a branch workflow where:

- `main` is protected and merge-only through pull requests,
- required status check for merge is `test-and-build-windows`,
- force-pushes and branch deletions on `main` are disabled,
- conversation resolution is required before merge.

Repository visibility is set to public for availability of branch protection/rules on the current GitHub plan.

## Rationale

- Preserves codebase stability as contributors and complexity increase.
- Ensures CI is always part of merge eligibility.
- Improves auditability and onboarding by making change history PR-driven.

## Consequences

- Every change must be developed on a feature branch.
- Contributors must wait for CI before merge.
- Workflow/configuration docs must be kept aligned with branch protection settings.
