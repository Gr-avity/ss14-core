# SS14 Code Placement

## Assembly Boundaries

- `Content.Shared`: data and logic both client and server may run; predicted gameplay belongs here when possible.
- `Content.Server`: authoritative-only logic, persistence, admin-only server work, atmos/power-only flows that cannot be predicted.
- `Content.Client`: rendering, XAML/BUI client windows, visualizers, overlays, input UI, client-only presentation.
- `Resources`: prototypes, maps, textures, audio, localization, guidebook data.
- `Content.Tests` and `Content.IntegrationTests`: focused validation.
- `RobustToolbox`: read-only for SS14-ART-CORE agents.

## SS14-ART-CORE Placement

New SS14-ART-CORE code must use `_Art` path segments:

- `Content.Shared/_Art/<Subsystem>/...`
- `Content.Server/_Art/<Subsystem>/...`
- `Content.Client/_Art/<Subsystem>/...`
- `Resources/Prototypes/_Art/<Subsystem>/...`
- `Resources/Locale/en-US/_Art/<Subsystem>/...`

Preserve SS14 folder semantics under `_Art`: `Components`, `EntitySystems`, `UI`, `Visualizers`, `Prototypes`, etc.

## Upstream Files

Only touch non-`_Art` files when integration requires it. Keep the diff narrow and apply `Art-Edit` markers around the exact changed lines.
