---
name: ss14-prototype-basics
description: SS14 YAML prototype, data field, localization, guidebook, entity table, reagent, and resource placement guidance. Use when editing Resources, prototypes, FTL, YAML, entity definitions, components serialized to YAML, or guidebook entries.
---

# SS14 Prototype Basics

## Placement

- New SS14-ART-CORE prototypes go in `Resources/Prototypes/_Art/<Subsystem>/`.
- New SS14-ART-CORE FTL goes in `Resources/Locale/en-US/_Art/<Subsystem>/`.
- Do not mix SS14-ART-CORE prototypes into upstream files unless integration requires it; then use `Art-Edit` markers.

## Bundled References

- `references/first-prototype-workflow.md`: add a new entity/prototype without drifting into upstream files.
- `references/common-prototype-components.md`: common component/resource/localization checks for SS14 prototypes.

## YAML Style

- Prototype block order: `type`, `abstract`, `parent`, `id`, `categories`, `name`, `suffix`, `description`, `components`.
- Use `PascalCase` for prototype IDs and component type names.
- Use `camelCase` for most YAML keys.
- Use inline lists for categories, regular lists elsewhere.
- Do not indent component list entries under `components:`.
- Separate prototypes by one blank line.

## Data Design

- Prefer prototypes over enums for in-game categories.
- In C#, use `ProtoId<T>` and typed prototype references.
- Avoid magic prototype IDs in logic.
- Use suffixes for spawn/admin differentiation.
- Keep abstract prototypes free of concrete sprite paths unless the inherited sprite is intentional.

## Localization

- Every player-facing string must be localized.
- Use kebab-case FTL IDs, scoped by subsystem.
- Entity prototype localization may use `ent-PrototypeId` and `.desc` where supported.

## Sources

See `ss14-wizden-docs` for YAML crash course, conventions, localization, guidebook entries, entity tables, reagents, and construction docs.
