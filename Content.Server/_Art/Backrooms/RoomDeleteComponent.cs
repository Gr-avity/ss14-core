using Robust.Shared.Prototypes;

namespace Content.Server._Art.Backrooms;

[RegisterComponent]
public sealed partial class RoomDeleteComponent : Component
{
    [DataField(required: true)]
    public EntProtoId ChunkLoaderProto;

    [DataField]
    public Vector2i RoomSize = new(5, 5);

    [DataField]
    public float PlayerCheckRadius = 12f;
}