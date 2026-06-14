using Content.Shared.Administration;
using Content.Shared.Doors.Components;
using Content.Shared._Art.Administration;
using Robust.Client.GameObjects;
using Robust.Shared.Console;

namespace Content.Client._Art.Administration
{
    public sealed class DoorMasterSystem : EntitySystem
    {
        [Dependency] private readonly SpriteSystem _spriteSystem = default!; // Для перекраски энтити

        public bool Enabled { get; set; } = false; // Для Toggle

        public override void Initialize()
        {
            base.Initialize();
            SubscribeNetworkEvent<ToggleDoorsEvent>(OnToggleDoors);
        }

        private void OnToggleDoors(ToggleDoorsEvent ev)
        {
            Toggle();
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            if (!Enabled) return;

            // Используем более специфичные запросы для оптимизации
            var queryOccluder = EntityQueryEnumerator<OccluderComponent, SpriteComponent>();
            while (queryOccluder.MoveNext(out var uid, out _, out var sprite))
            {
                if (sprite.Color.A > 0.31f)
                {
                    _spriteSystem.SetColor(uid, sprite.Color.WithAlpha(0.3f));
                }
            }

            var queryDoor = EntityQueryEnumerator<DoorComponent, SpriteComponent>();
            while (queryDoor.MoveNext(out var uid, out _, out var sprite))
            {
                if (sprite.Color.A > 0.31f)
                {
                    _spriteSystem.SetColor(uid, sprite.Color.WithAlpha(0.3f));
                }
            }
        }

        public void Toggle()
        {
            Enabled = !Enabled;

            var alpha = Enabled ? 0.3f : 1.0f;

            var queryOccluder = EntityQueryEnumerator<OccluderComponent, SpriteComponent>();
            while (queryOccluder.MoveNext(out var uid, out _, out var sprite))
            {
                _spriteSystem.SetColor(uid, sprite.Color.WithAlpha(alpha));
            }

            var queryDoor = EntityQueryEnumerator<DoorComponent, SpriteComponent>();
            while (queryDoor.MoveNext(out var uid, out _, out var sprite))
            {
                _spriteSystem.SetColor(uid, sprite.Color.WithAlpha(alpha));
            }
        }
    }
}
