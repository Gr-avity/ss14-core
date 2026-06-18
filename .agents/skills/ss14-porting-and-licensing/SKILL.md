---
name: ss14-porting-and-licensing
description: Porting, attribution, fork isolation, upstream merge, and license guidance for SS14-ART-CORE. Use before importing code/assets from WizDen, Delta-V, Starlight,
or other SS14 forks, assets, or external repositories, and before modifying upstream files.
---

# SS14 Porting And Licensing

## Before Porting

- Identify source repository, commit, original author, and license.
- Check whether code/assets are MIT, AGPL, MPL, CC-BY-SA, CC-BY-NC-SA, or another license.
- Do not import incompatible assets or hidden-source-incompatible code.
- Preserve attribution in metadata, comments, or license files as appropriate.

## Fork Isolation

- Put new SS14-ART-CORE code/assets under `_Art`.
- Namespace serialized types and prototype IDs with a SS14-ART-CORE prefix.
- Avoid changing upstream files. If required, use tight `Art-Edit` markers.
- Preserve path similarity under `_Art` so upstream equivalents are easy to compare.

## Database Porting

- Avoid modifying upstream tables.
- Prefer one-to-one fork-owned tables for SS14-ART-CORE-only data.
- Namespace migrations.
- Test SQLite and Postgres paths when persistence changes.

## Bundled References

- `references/source-license-checklist.md`: source, commit, author, license, compatibility, and import decision checks.
- `references/attribution-patterns.md`: code, asset, RSI, station image, and generic attribution placement.

## Sources

See `ss14-wizden-docs` for forking tips, PRs with engine changes, generic attribution, station image specs, and PR guidelines.
