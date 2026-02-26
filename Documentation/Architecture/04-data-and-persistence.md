# Data and Persistence Model

## Persistence Principle

Persist gameplay domain state, not transient Unity scene state.

## Current Baseline

- Persistence package currently contains placeholder storage interfaces/classes.
- Serialization/migration implementation is expected to evolve in this package.

## Data Design Constraints

1. Save formats must be versioned.
2. Backward compatibility strategy must be documented before changing save schema.
3. Migration logic should be deterministic and test-covered.

## Recommended Save Pipeline

1. Convert domain state to DTO snapshot.
2. Add metadata (`version`, `timestamp`, optional checksum).
3. Serialize to chosen format.
4. Write through storage abstraction.
5. On load, run migration chain before domain reconstruction.

## Documentation Requirement

When adding/changing save schema, update this file and create an ADR if compatibility trade-offs are involved.
