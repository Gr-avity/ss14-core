# WizDen Docs Notes For SS14-ART-CORE Skills

These are distilled notes from the local WizDen docs. They are intentionally shorter than the docs and tuned for SS14-ART-CORE code review.

## Fork Work

- Do not use GitHub's fork button as the core architectural model for a long-lived fork; keep remotes and upstream merge flow explicit.
- Keep fork code in dedicated folders inside major code sections.
- Namespace serialized types, prototypes, CVars, and migrations.
- Merge conflicts are useful signals; do not hide them with brittle avoidance code.
- Prefer fork-owned DB tables over modifying upstream tables.
- Test DB changes on SQLite and Postgres.
- Licenses matter for code and assets; check source, commit, author, license, and attribution.

SS14-ART-CORE-specific override: new SS14-ART-CORE code goes under `_Art`; upstream-file edits need `Art-Edit` markers; `RobustToolbox/**` is read-only.

## Code Organization

- `Content.Shared` enables prediction and shared data/contracts.
- `Content.Server` is authoritative and server-only.
- `Content.Client` is presentation, visualizers, UI, and client-only behavior.
- Resources include prototypes, maps, textures, audio, localization, guidebook.
- Organize game code by subsystem, not "misc".
- Under `_Art`, preserve this organization so future upstream comparison remains easy.

## ECS

- Components are data containers and markers.
- Systems own behavior.
- Use composition over inheritance for gameplay.
- Use system public APIs instead of writing component fields from anywhere.
- Events decouple systems; directed events are preferred when entity-specific.
- Method events are normally prohibited unless wrapped by a system method.

## C# Style

- Use file-scoped namespaces.
- Fields and auto-properties come first.
- Classes must be `sealed`, `abstract`, `static`, or `[Virtual]`.
- Avoid copy-paste, magic strings, and magic numbers.
- Comment why the code exists, not obvious syntax.
- Use XML docs for public APIs and non-obvious DataFields.
- Use `TimeSpan`, `IGameTiming.CurTime`, pause support, and `TimeOffsetSerializer` for runtime timers.

## EntitySystem APIs

- Public game APIs should take `Entity<T?>` or `EntityUid` first.
- Resolve components at method start.
- Guard no-op setter calls before mutating and dirtying.
- Use dependencies for systems/managers.
- Prefer EntitySystem proxy methods over raw `EntityManager` where available.
- Use `ToPrettyString` in admin logs.

## Prototypes And YAML

- Prefer prototypes over enums for in-game types.
- Use `ProtoId<T>` for prototype ID fields.
- Do not cache prototype objects unless current local code has a clear reason.
- YAML prototype order: type, abstract, parent, id, categories, name, suffix, description, components.
- Component list items under `components:` are not extra-indented.
- Use PascalCase IDs and component names; use camelCase YAML keys.
- Use suffix for spawn-menu distinction.

## Localization

- Every player-facing string is localized.
- Use Fluent variables instead of concatenation.
- Use entity grammar helpers where appropriate.
- Localization IDs are kebab-case and subsystem-scoped.
- Put SS14-ART-CORE FTL under `Resources/Locale/en-US/_Art`.

## Prediction And Networking

- New interactive code should be predicted when possible.
- Shared predicted components require `[NetworkedComponent]`, `[AutoGenerateComponentState]`, and `[AutoNetworkedField]`.
- Dirty changed networked fields; use field deltas when independent fields change often.
- Use predicted popup/audio/spawn/delete/BUI message APIs.
- Client prediction may run code many times; guard non-idempotent effects.
- Client network events are untrusted; validate server-side.
- Use PVS/session-specific tools to avoid leaking hidden data.
- Avoid `[NetworkedComponent]` outside `Content.Shared`.

## UI

- Prefer XAML for new UI layout.
- Use existing style classes and controls.
- BUI is for entity-bound UI; EUI is for server/admin/service-style UI.
- Server validates BUI/EUI actions.
- For predicted BUI, avoid duplicate BUI state when component state already contains the data.

## Rendering

- Use `SpriteSpecifier` for configurable sprite fields.
- Prefer `AppearanceComponent` and `GenericVisualizer` for data-driven visuals.
- RSI metadata must preserve license/copyright and formatting.
- Client visual code belongs in client; shared data belongs in shared.

## Transform, Physics, Maps

- Use transform system APIs for anchoring and placement.
- Prefer map/grid/entity coordinate-aware APIs over raw vectors.
- Do not invent collision bitmasks.
- Check PVS/nullspace behavior for entity references.
- Mapping work must validate atmos, power, cameras, telecoms, docking, arrivals, evac, spawn points, and prototype availability.

## Domain Design

- SS14 design values: chaos, seriously silly tone, dynamic environment, intuitive interconnected simulation, player interaction, player agency.
- Department docs define intended responsibilities and undesired gameplay; use them to avoid feature creep.
- Species and antagonist docs emphasize balance, admin burden, accessibility, and technical feasibility.
- Proposals should explain overview, background, gameplay/roundflow impact, admin impact, and technical considerations.
