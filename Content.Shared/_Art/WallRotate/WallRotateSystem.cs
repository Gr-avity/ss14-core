using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Shared._Art.WallAdjacentRotate;

public sealed class WallRotateSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<WallAdjacentRotateComponent, AnchorStateChangedEvent>(OnAnchorChanged);
        SubscribeLocalEvent<WallAdjacentRotateComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(EntityUid uid, WallAdjacentRotateComponent comp, MapInitEvent args)
    {
        TryAutoRotate(uid);
    }

    private void OnAnchorChanged(EntityUid uid, WallAdjacentRotateComponent comp, ref AnchorStateChangedEvent args)
    {
        if (!args.Anchored)
            return;
        TryAutoRotate(uid);
    }

    private void TryAutoRotate(EntityUid uid)
    {
	    if (!TryComp<WallAdjacentRotateComponent>(uid, out var comp) || !comp.Enabled)
        return;
	
        var xform = Transform(uid);
        if (!xform.Anchored || xform.GridUid is not { } gridUid)
            return;

        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return;

        var tile = _mapSystem.LocalToTile(gridUid, grid, xform.Coordinates);

        bool north = HasWallConnectable(gridUid, grid, tile + new Vector2i(0, 1));
        bool south = HasWallConnectable(gridUid, grid, tile + new Vector2i(0, -1));
        bool east  = HasWallConnectable(gridUid, grid, tile + new Vector2i(1, 0));
        bool west  = HasWallConnectable(gridUid, grid, tile + new Vector2i(-1, 0));

        if (north && south && !east && !west)
        {
            _transform.SetLocalRotation(uid, Math.PI / 2);
            return;
        }

        if (east && west && !north && !south)
        {
            _transform.SetLocalRotation(uid, 0);
            return;
        }

        var nsCount = (north ? 1 : 0) + (south ? 1 : 0);
        var ewCount = (east ? 1 : 0) + (west ? 1 : 0);
        if (nsCount > ewCount)
            _transform.SetLocalRotation(uid, Math.PI / 2);
        else if (ewCount > nsCount)
            _transform.SetLocalRotation(uid, 0);
    }

    private bool HasWallConnectable(EntityUid gridUid, MapGridComponent grid, Vector2i tile) 
    {
        var enumerator = _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, tile);
        while (enumerator.MoveNext(out var ent))
        {
            if (HasComp<WallConnectableComponent>(ent.Value))
                return true;
        }
        return false;
    }
}