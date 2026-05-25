using Robust.Shared.Configuration;

namespace Content.Shared._Art.CCVar;

[CVarDefs]
public sealed partial class SS14ArtCCvar
{
    public static readonly CVarDef<string> AuthApiUrl =
        CVarDef.Create("discord_auth.url", "", CVar.SERVERONLY);

    public static readonly CVarDef<string> AuthApiToken =
        CVarDef.Create("discord_auth.token", "key", CVar.SERVERONLY);

    public static readonly CVarDef<string> AuthTargetGuild =
        CVarDef.Create("discord_auth.guild", "", CVar.SERVERONLY);
}