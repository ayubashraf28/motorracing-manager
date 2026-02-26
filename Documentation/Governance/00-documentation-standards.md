# Documentation Standards and Governance

## Objective

Keep documentation continuously accurate, discoverable, and implementation-aligned as project complexity grows.

## Rules

1. Documentation updates are required in the same PR as code changes.
2. Architectural decisions must be captured in `ADR/` when trade-offs are involved.
3. Avoid speculative docs; describe implemented behavior and explicitly mark planned behavior.
4. Prefer concise sections with stable headings over long narrative prose.
5. Changes to branch workflow/protection must update `Development/` and `Operations/` documentation in the same PR.

## Ownership

- All contributors are responsible for keeping touched areas documented.
- Reviewer responsibility includes documentation correctness, not only code correctness.

## Review Cadence

- Per PR: ensure relevant doc files were updated.
- Per milestone: run a documentation consistency pass across Architecture/Development/Operations.

## Definition of Done (Documentation)

A change is not done until:

- behavior is implemented,
- tests are updated,
- and documentation reflects the new reality.
