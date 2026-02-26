# Development Workflow and Documentation Updates

## Branch and PR Workflow

1. Create a feature branch from `main`.
2. Implement code and tests.
3. Update related docs in `Documentation/`.
4. Open PR with architecture/testing notes.
5. Merge only after CI passes.

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

## Change Note Format

In PR description include:

- What changed.
- Why it changed.
- Which documentation files were updated.
- Any follow-up documentation debt.
