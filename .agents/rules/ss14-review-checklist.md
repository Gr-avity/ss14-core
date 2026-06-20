# SS14 Review Checklist

Review AI-generated SS14-ART-CORE changes against this list.

- No `RobustToolbox/**` modifications.
- New SS14-ART-CORE code lives under `_Art`.
- Any non-`_Art` file change has a tight `Art-Edit` marker block.
- Code is modular, data-driven, and does not duplicate an existing mechanic.
- Component data stays data-only; behavior is in systems.
- Shared/server/client placement matches prediction and security needs.
- Networked fields are dirtied correctly and do not leak hidden info.
- Client network input is validated server-side.
- Player-facing text is localized.
- Prototype IDs, sounds, sprites, CVars, and magic values are typed/configurable.
- Tests or manual validation cover the real behavior, especially prediction and networking.
- PR explains linked issue, balance impact, technical details, media, and licensing/attribution.
