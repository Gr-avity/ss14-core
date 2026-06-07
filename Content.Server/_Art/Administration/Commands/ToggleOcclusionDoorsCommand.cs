using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared._Art.Administration;
using Robust.Shared.Console;
using Robust.Shared.Network;

namespace Content.Server._Art.Administration
{
    [AdminCommand(AdminFlags.Mapping)]
    public sealed class ToggleDoorsCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityNetworkManager _net = default!;

        public string Command => "toggledoors";
        public string Description => Loc.GetString("cmd-toggledoors-desc");
        public string Help => Loc.GetString("cmd-toggledoors-help");

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            _net.SendSystemNetworkMessage(new ToggleDoorsEvent());
        }
    }
}
