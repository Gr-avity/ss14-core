---
name: ss14-upstream-maintenance
description: Upstream/fork maintenance guidance for SS14-ART-CORE. Use when touching non-_Art files, reviewing upstream diffs, preserving path similarity, porting fork code, marking ss14-art-edit blocks, or deciding whether an engine/content change is allowed.
---

# SS14 Upstream Maintenance

This skill protects SS14-ART-CORE from painful upstream merges.

## Workflow

1. Open `references/engine-boundaries.md`.
2. Open `references/fork-only-content.md`.
3. Open `references/edit-strategy.md`.
4. Open `references/edit-types.md` and `references/path-similarity.md` for ports.

## Rules

- Never edit `RobustToolbox`.
- Prefer `_Art` fork-owned files.
- Mark every non-`_Art` change with narrow `Art-Edit` blocks.
- Keep upstream diffs small and intentional.
- Preserve upstream path similarity for ports so future merges and blame remain readable.
- Do not use conflict-avoidance hacks to hide meaningful upstream behavior changes.

## Marker Format

Use native comments and keep the block as small as possible:

```csharp
// Art-Start: reason
CODE
// Art-End
```
