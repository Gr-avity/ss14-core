using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization;

namespace Content.Shared._Art.Chemistry;

// SS14-Art edit start
[Serializable, NetSerializable]
public sealed class ChemAnalyzeReagentMessage : BoundUserInterfaceMessage
{
    public readonly ReagentId ReagentId;

    public ChemAnalyzeReagentMessage(ReagentId reagentId)
    {
        ReagentId = reagentId;
    }
}

[Serializable, NetSerializable]
public sealed class ChemPrintAnalysisMessage : BoundUserInterfaceMessage
{
    public readonly ReagentId ReagentId;

    public ChemPrintAnalysisMessage(ReagentId reagentId)
    {
        ReagentId = reagentId;
    }
}

[Serializable, NetSerializable]
public sealed class ChemReagentAnalysisPopupMessage : BoundUserInterfaceMessage
{
    public readonly ReagentId ReagentId;
    public readonly string Title;
    public readonly string Description;

    public ChemReagentAnalysisPopupMessage(ReagentId reagentId, string title, string description)
    {
        ReagentId = reagentId;
        Title = title;
        Description = description;
    }
}
// SS14-Art edit end
