using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
    /// <summary>
    /// This class holds constants that are shared between client and server.
    /// </summary>
    public sealed class SharedReagentDispenser
    {
        public const string OutputSlotName = "beakerSlot";

        // SS14-Art edit start
        public const float DefaultEnergyCapacity = 1000f;
        public const float DefaultRechargePerSecond = 2f;
        // SS14-Art edit end
    }

    [Serializable, NetSerializable]
    public sealed class ReagentDispenserSetDispenseAmountMessage : BoundUserInterfaceMessage
    {
        public readonly ReagentDispenserDispenseAmount ReagentDispenserDispenseAmount;

        public ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount amount)
        {
            ReagentDispenserDispenseAmount = amount;
        }

        public ReagentDispenserSetDispenseAmountMessage(string s)
        {
            switch (s)
            {
                case "1":   ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U1;   break;
                case "5":   ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U5;   break;
                case "10":  ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U10;  break;
                case "15":  ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U15;  break;
                case "20":  ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U20;  break;
                case "25":  ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U25;  break;
                case "30":  ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U30;  break;
                case "50":  ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U50;  break;
                case "100": ReagentDispenserDispenseAmount = ReagentDispenserDispenseAmount.U100; break;
                default:
                    throw new Exception($"Cannot convert the string `{s}` into a valid ReagentDispenser DispenseAmount");
            }
        }
    }

    [Serializable, NetSerializable]
    public sealed class ReagentDispenserDispenseReagentMessage : BoundUserInterfaceMessage
    {
        // SS14-Art edit start
        public readonly ReagentId ReagentId;

        public ReagentDispenserDispenseReagentMessage(ReagentId reagentId)
        {
            ReagentId = reagentId;
        }
        // SS14-Art edit end
    }

    [Serializable, NetSerializable]
    public sealed class ReagentDispenserContainerActionMessage : BoundUserInterfaceMessage
    {
        // SS14-Art edit start
        public readonly ReagentId ReagentId;
        public readonly ReagentDispenserContainerAction Action;
        public readonly FixedPoint2 Quantity;

        public ReagentDispenserContainerActionMessage(ReagentId reagentId, ReagentDispenserContainerAction action, FixedPoint2 quantity = default)
        {
            ReagentId = reagentId;
            Action = action;
            Quantity = quantity;
        }
        // SS14-Art edit end
    }

    // SS14-Art edit start
    [Serializable, NetSerializable]
    public enum ReagentDispenserContainerAction : byte
    {
        Spill,
        SpillAll,
        Delete,
        Analyze,
    }
    // SS14-Art edit end

    [Serializable, NetSerializable]
    public sealed class ReagentDispenserClearContainerSolutionMessage : BoundUserInterfaceMessage
    {
    }

    // Starlight-start: Plumbing valve toggle
    [Serializable, NetSerializable]
    public sealed class ReagentDispenserToggleValveMessage : BoundUserInterfaceMessage
    {
    }
    // Starlight-end

    public enum ReagentDispenserDispenseAmount
    {
        U1   = 1,
        U5   = 5,
        U10  = 10,
        U15  = 15,
        U20  = 20,
        U25  = 25,
        U30  = 30,
        U50  = 50,
        U100 = 100,
    }

    // SS14-Art edit start
    [Serializable, NetSerializable]
    public sealed class ReagentDispenserInventoryItem
    {
        public ReagentId ReagentId;
        public string ReagentLabel;
        public Color ReagentColor;

        public ReagentDispenserInventoryItem(ReagentId reagentId, string reagentLabel, Color reagentColor)
        {
            ReagentId = reagentId;
            ReagentLabel = reagentLabel;
            ReagentColor = reagentColor;
        }
    }
    // SS14-Art edit end

    [Serializable, NetSerializable]
    public sealed class ReagentDispenserBoundUserInterfaceState : BoundUserInterfaceState
    {
        public readonly ContainerInfo? OutputContainer;
        public readonly NetEntity? OutputContainerEntity;

        // SS14-Art edit start
        public readonly List<ReagentDispenserInventoryItem> Inventory;
        public readonly float CurrentEnergy;
        public readonly float MaxEnergy;
        // SS14-Art edit end

        public readonly ReagentDispenserDispenseAmount SelectedDispenseAmount;

        // Starlight-edit: Plumbing valve
        public readonly bool ValveOpen;

        public ReagentDispenserBoundUserInterfaceState(
            ContainerInfo? outputContainer,
            NetEntity? outputContainerEntity,
            List<ReagentDispenserInventoryItem> inventory,
            ReagentDispenserDispenseAmount selectedDispenseAmount,
            float currentEnergy,
            float maxEnergy,
            bool valveOpen)
        {
            OutputContainer = outputContainer;
            OutputContainerEntity = outputContainerEntity;
            Inventory = inventory;
            SelectedDispenseAmount = selectedDispenseAmount;
            CurrentEnergy = currentEnergy;
            MaxEnergy = maxEnergy;
            ValveOpen = valveOpen;
        }
    }

    [Serializable, NetSerializable]
    public enum ReagentDispenserUiKey
    {
        Key
    }
}
