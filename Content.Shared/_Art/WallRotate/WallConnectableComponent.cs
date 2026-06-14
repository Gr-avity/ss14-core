using Robust.Shared.GameStates;

namespace Content.Shared._Art.WallAdjacentRotate;

[RegisterComponent, NetworkedComponent]
public sealed partial class WallConnectableComponent : Component
{
    [DataField]
    public bool Enabled = true;
}