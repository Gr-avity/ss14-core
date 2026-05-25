using Robust.Shared.Configuration;

namespace Content.Shared._White;

[CVarDefs]
public sealed class WhiteCVars
{
    public static readonly CVarDef<bool> PixelSnapCamera =
    	CVarDef.Create("experimental.pixel_snap_camera", false, CVar.CLIENTONLY | CVar.ARCHIVE);
}
