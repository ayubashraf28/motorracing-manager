# Coding Standards

## Language and Layering

- Keep engine-free logic in `Core/Domain/Sim/App/Persistence`.
- Keep `Unity` and `UI` focused on adaptation and presentation.
- Prefer explicit interfaces at layer boundaries.

## C# Guidelines

- Use clear names for use-cases and domain concepts.
- Keep classes cohesive and small.
- Avoid hidden global state.
- Prefer constructor or composition-root injection over static access.

## Unity-Specific Guidelines

- `MonoBehaviour` should orchestrate, not decide business rules.
- Avoid direct cross-layer shortcuts from UI to persistence.
- Keep scene scripts minimal and observable through logs/events.

## Assembly Definition Hygiene

- Add only minimal references required.
- Preserve intended dependency direction.
- Keep test asmdefs isolated with explicit references.

## Comment and Documentation Standard

- Add comments only where intent is non-obvious.
- Keep public APIs documented through concise XML comments when needed.
- Update architecture docs alongside non-trivial structural changes.
