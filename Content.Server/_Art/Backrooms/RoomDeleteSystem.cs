using System.Numerics;
using Content.Shared.Maps;
using Content.Shared.Mind.Components;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;

namespace Content.Server._Art.Backrooms;

public sealed class RoomDeleteSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedMapSystem _maps = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private readonly HashSet<Entity<MindContainerComponent>> _players = new();
    private readonly List<(Vector2i, Tile)> _tiles = new();
    private readonly HashSet<EntityUid> _ents = new();
    private readonly List<EntityUid> _toDelete = new();

    private float _checkAccumulator;
    private const float CheckInterval = 15f;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _checkAccumulator += frameTime;
        if (_checkAccumulator < CheckInterval)
            return;

        _checkAccumulator = 0f;

        var query = EntityQueryEnumerator<RoomDeleteComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var tracker, out var xform))
        {
            _players.Clear();
            _lookup.GetEntitiesInRange(xform.Coordinates, tracker.PlayerCheckRadius, _players);

            var hasPlayer = false;
            foreach (var player in _players)
            {
                if (player.Comp.HasMind)
                {
                    hasPlayer = true;
                    break;
                }
            }

            if (hasPlayer)
                continue;

            ClearRoom(uid, tracker, xform);
            Spawn(tracker.ChunkLoaderProto, xform.Coordinates);
            QueueDel(uid);
        }
    }

    private void ClearRoom(EntityUid uid, RoomDeleteComponent tracker, TransformComponent xform)
    {
        if (xform.GridUid == null)
            return;

        var gridUid = xform.GridUid.Value;
        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return;

        var center = _maps.LocalToTile(gridUid, grid, xform.Coordinates);
        var halfX = tracker.RoomSize.X / 2;
        var halfY = tracker.RoomSize.Y / 2;

        _tiles.Clear();

        for (var x = center.X - halfX; x <= center.X + halfX; x++)
        {
            for (var y = center.Y - halfY; y <= center.Y + halfY; y++)
            {
                var tilePos = new Vector2i(x, y);
                var tileRef = _maps.GetTileRef(gridUid, grid, tilePos);

                _ents.Clear();
                _lookup.GetEntitiesInTile(tileRef, _ents, LookupFlags.Dynamic | LookupFlags.Static | LookupFlags.Sundries);
                _toDelete.Clear();
                foreach (var ent in _ents)
                {
                    if (ent == uid)
                        continue;
                    if (TryComp<MindContainerComponent>(ent, out var mind) && mind.HasMind)
                        continue;
                    _toDelete.Add(ent);
                }
                foreach (var ent in _toDelete)
                    QueueDel(ent);

                _tiles.Add((tilePos, Tile.Empty));
            }
        }
        _maps.SetTiles(gridUid, grid, _tiles);
    }
}