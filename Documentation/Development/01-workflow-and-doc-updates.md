# Development Workflow and Documentation Updates

## Branch and PR Workflow

1. Create a feature branch from `main`.
2. Implement code and tests.
3. Update related docs in `Documentation/`.
4. Push feature branch and open PR with architecture/testing notes.
5. Merge after required policy checks pass (PR policy always; CI-gating policy per current branch protection state).

## Protected Branch Policy

- Direct pushes to `main` are not allowed.
- Work is expected to flow as `feature/* -> PR -> main`.
- Required status check for merge: temporarily optional while Unity activation is being fixed.
- Restoration condition: re-enable required `test-and-build-windows` after activation is validated by a successful CI run.

## Mandatory Documentation Rule

Every PR must update documentation when it changes one of the following:

- architecture/layer dependencies.
- public interfaces/contracts.
- data model or save format.
- build/test/release behavior.
- developer setup requirements.

## PR Checklist (Documentation)

- `Documentation/README.md` still reflects section map.
- Relevant architecture/development/operations file is updated.
- New decision with trade-off is captured in `ADR/`.
- Terminology stays consistent with `Product` docs.
- If repo workflow/governance changed, update `Operations/00-ci-cd.md` and this file.

## Change Note Format

In PR description include:

- What changed.
- Why it changed.
- Which documentation files were updated.
- Any follow-up documentation debt.
