using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
    /// <summary>
    /// This class holds constants that are shared between client and server.
    /// </summary>
    public sealed class SharedChemMaster
    {
        public const uint PillTypes = 20;
        public const string BufferSolutionName = "buffer";
        public const string InputSlotName = "beakerSlot";
        public const string OutputSlotName = "outputSlot";
        public const string PillSolutionName = "food";
        public const string PatchSolutionName = "patch"; // Starlight-edit
        public const string BottleSolutionName = "drink";
        public const uint LabelMaxLength = 150; // Increased to support more reagents
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterSetModeMessage : BoundUserInterfaceMessage
    {
        public readonly ChemMasterMode ChemMasterMode;

        public ChemMasterSetModeMessage(ChemMasterMode mode)
        {
            ChemMasterMode = mode;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterSetPillTypeMessage : BoundUserInterfaceMessage
    {
        public readonly uint PillType;

        public ChemMasterSetPillTypeMessage(uint pillType)
        {
            PillType = pillType;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterReagentAmountButtonMessage : BoundUserInterfaceMessage
    {
        public readonly ReagentId ReagentId;
        public readonly int Amount;
        public readonly bool FromBuffer;
        public readonly bool IsOutput;

        public ChemMasterReagentAmountButtonMessage(ReagentId reagentId, int amount, bool fromBuffer, bool isOutput)
        {
            ReagentId = reagentId;
            Amount = amount;
            FromBuffer = fromBuffer;
            IsOutput = isOutput;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterCreatePillsMessage : BoundUserInterfaceMessage
    {
        public readonly uint Dosage;
        public readonly uint Number;
        public readonly string Label;

        public ChemMasterCreatePillsMessage(uint dosage, uint number, string label)
        {
            Dosage = dosage;
            Number = number;
            Label = label;
        }
    }

    // Starlight-start
    [Serializable, NetSerializable]
    public sealed class ChemMasterCreatePatchesMessage : BoundUserInterfaceMessage
    {
        public readonly uint Dosage;
        public readonly uint Number;
        public readonly string Label;

        public ChemMasterCreatePatchesMessage(uint dosage, uint number, string label)
        {
            Dosage = dosage;
            Number = number;
            Label = label;
        }
    }
    // Starlight-end

    [Serializable, NetSerializable]
    public sealed class ChemMasterOutputToBottleMessage : BoundUserInterfaceMessage
    {
        public readonly uint Dosage;
        public readonly uint Number;
        public readonly string Label;

        public ChemMasterOutputToBottleMessage(uint dosage, uint number, string label)
        {
            Dosage = dosage;
            Number = number;
            Label = label;
        }
    }

    // Messages for Handlers
    [Serializable, NetSerializable]
    public sealed class ChemMasterSortMethodUpdated : BoundUserInterfaceMessage
    {
        public readonly int SortMethod;

        public ChemMasterSortMethodUpdated(int sortMethod)
        {
            SortMethod = sortMethod;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterTransferringAmountUpdated : BoundUserInterfaceMessage
    {
        public readonly int TransferringAmount;

        public ChemMasterTransferringAmountUpdated(int transferringAmount)
        {
            TransferringAmount = transferringAmount;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterAmountsUpdated : BoundUserInterfaceMessage
    {
        public readonly List<int> Amounts;

        public ChemMasterAmountsUpdated(List<int> amounts)
        {
            Amounts = amounts;
        }
    }

    // Bottle buttons reagent transfer
    [Serializable, NetSerializable]
    public sealed class ChemMasterChooseReagentMessage : BoundUserInterfaceMessage
    {
        public ReagentId Reagent;

        public ChemMasterChooseReagentMessage(ReagentId reagent)
        {
            Reagent = reagent;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterClearReagentSelectionMessage : BoundUserInterfaceMessage
    {
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterToggleBottleFillMessage : BoundUserInterfaceMessage
    {
        public int Slot;

        public ChemMasterToggleBottleFillMessage(int slot)
        {
            Slot = slot;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterRowEjectMessage : BoundUserInterfaceMessage
    {
        public readonly int Row;

        public ChemMasterRowEjectMessage(int row)
        {
            Row = row;
        }
    }

    // Pill container messages
    [Serializable, NetSerializable]
    public sealed class ChemMasterSelectPillContainerSlotMessage : BoundUserInterfaceMessage
    {
        public readonly int Slot;

        public ChemMasterSelectPillContainerSlotMessage(int slot)
        {
            Slot = slot;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterTogglePillContainerFillMessage : BoundUserInterfaceMessage
    {
        public readonly int Slot;

        public ChemMasterTogglePillContainerFillMessage(int slot)
        {
            Slot = slot;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterPillContainerSlotEjectMessage : BoundUserInterfaceMessage
    {
        public readonly int Slot;

        public ChemMasterPillContainerSlotEjectMessage(int slot)
        {
            Slot = slot;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterPillContainerRowEjectMessage : BoundUserInterfaceMessage
    {
        public readonly int Row;

        public ChemMasterPillContainerRowEjectMessage(int row)
        {
            Row = row;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterSelectPillCanisterForCreationMessage : BoundUserInterfaceMessage
    {
        public readonly int CanisterIndex;

        public ChemMasterSelectPillCanisterForCreationMessage(int canisterIndex)
        {
            CanisterIndex = canisterIndex;
        }
    }

    // Reagent amount selection messages
    [Serializable, NetSerializable]
    public sealed class ChemMasterSelectReagentAmountMessage : BoundUserInterfaceMessage
    {
        public readonly ReagentId Reagent;
        public readonly int Amount;

        public ChemMasterSelectReagentAmountMessage(ReagentId reagent, int amount)
        {
            Reagent = reagent;
            Amount = amount;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterRemoveReagentAmountMessage : BoundUserInterfaceMessage
    {
        public readonly ReagentId Reagent;
        public readonly int Amount;

        public ChemMasterRemoveReagentAmountMessage(ReagentId reagent, int amount)
        {
            Reagent = reagent;
            Amount = amount;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterClearReagentAmountMessage : BoundUserInterfaceMessage
    {
        public readonly ReagentId Reagent;

        public ChemMasterClearReagentAmountMessage(ReagentId reagent)
        {
            Reagent = reagent;
        }
    }

    // Starlight-start: Plumbing valve toggle
    [Serializable, NetSerializable]
    public sealed class ChemMasterToggleValveMessage : BoundUserInterfaceMessage
    {
    }
    // Starlight-end

    public enum ChemMasterMode
    {
        Transfer,
        Discard,
    }

    /// <summary>
    /// Information about the capacity and contents of a container for display in the UI
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class ContainerInfo
    {
        /// <summary>The container name to show to the player</summary>
        public readonly string DisplayName;

        /// <summary>The currently used volume of the container</summary>
        public readonly FixedPoint2 CurrentVolume;

        /// <summary>The maximum volume of the container</summary>
        public readonly FixedPoint2 MaxVolume;

        /// <summary>A list of the entities and their sizes within the container</summary>
        public List<(string Id, FixedPoint2 Quantity)>? Entities { get; init; }

        public List<ReagentQuantity>? Reagents { get; init; }

        public ContainerInfo(string displayName, FixedPoint2 currentVolume, FixedPoint2 maxVolume)
        {
            DisplayName = displayName;
            CurrentVolume = currentVolume;
            MaxVolume = maxVolume;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterBoundUserInterfaceState : BoundUserInterfaceState
    {
        public readonly ContainerInfo? ContainerInfo;
        public readonly IReadOnlyList<ReagentQuantity> BufferReagents;
        public readonly ChemMasterMode Mode;
        public readonly FixedPoint2? BufferCurrentVolume;
        public readonly uint SelectedPillType;
        public readonly uint PillDosageLimit;
        public readonly uint PatchDosageLimit; // Starlight-edit
        public readonly uint BottleDosageLimit;
        public readonly uint MaxPills;
        public readonly uint MaxBottles;
        public readonly bool UpdateLabel;
        public readonly int SortMethod;
        public readonly int TransferringAmount;
        public readonly List<int> Amounts;

        // Pill container storage
        public readonly List<ContainerInfo?> StoredPillContainers;
        public readonly List<List<bool>> PillContainers;
        public readonly List<List<uint>> PillTypes;
        public readonly int SelectedPillContainerSlot;
        public readonly int SelectedPillContainerForFill;
        public readonly int SelectedPillCanisterForCreation;
        public readonly ReagentId? SelectedReagent;
        public readonly ContainerInfo? SelectedPillContainerInfo;

        // Bottle container storage
        public readonly List<ContainerInfo?> StoredBottles;
        public readonly int SelectedBottleForFill;
        public readonly List<ReagentId> SelectedReagentsForBottles;
        public readonly Dictionary<ReagentId, float> SelectedReagentAmounts;

        // Starlight: valve
        public readonly bool ValveOpen;

        public ChemMasterBoundUserInterfaceState(
            ChemMasterMode mode,
            ContainerInfo? containerInfo,
            IReadOnlyList<ReagentQuantity> bufferReagents,
            FixedPoint2 bufferCurrentVolume,
            uint selectedPillType,
            uint pillDosageLimit,
            uint patchDosageLimit,
            uint bottleDosageLimit,
            uint maxPills,
            uint maxBottles,
            bool updateLabel,
            int sortMethod,
            int transferringAmount,
            List<int> amounts,
            List<ContainerInfo?> storedPillContainers,
            List<List<bool>> pillContainers,
            List<List<uint>> pillTypes,
            int selectedPillContainerSlot,
            int selectedPillContainerForFill,
            int selectedPillCanisterForCreation,
            ReagentId? selectedReagent,
            ContainerInfo? selectedPillContainerInfo,
            List<ContainerInfo?> storedBottles,
            int selectedBottleForFill,
            List<ReagentId> selectedReagentsForBottles,
            Dictionary<ReagentId, float> selectedReagentAmounts,
            bool valveOpen)
        {
            Mode = mode;
            ContainerInfo = containerInfo;
            BufferReagents = bufferReagents;
            BufferCurrentVolume = bufferCurrentVolume;
            SelectedPillType = selectedPillType;
            PillDosageLimit = pillDosageLimit;
            PatchDosageLimit = patchDosageLimit;
            BottleDosageLimit = bottleDosageLimit;
            MaxPills = maxPills;
            MaxBottles = maxBottles;
            UpdateLabel = updateLabel;
            SortMethod = sortMethod;
            TransferringAmount = transferringAmount;
            Amounts = amounts;
            StoredPillContainers = storedPillContainers;
            PillContainers = pillContainers;
            PillTypes = pillTypes;
            SelectedPillContainerSlot = selectedPillContainerSlot;
            SelectedPillContainerForFill = selectedPillContainerForFill;
            SelectedPillCanisterForCreation = selectedPillCanisterForCreation;
            SelectedReagent = selectedReagent;
            SelectedPillContainerInfo = selectedPillContainerInfo;
            StoredBottles = storedBottles;
            SelectedBottleForFill = selectedBottleForFill;
            SelectedReagentsForBottles = selectedReagentsForBottles;
            SelectedReagentAmounts = selectedReagentAmounts;
            ValveOpen = valveOpen;
        }
    }

    [Serializable, NetSerializable]
    public enum ChemMasterUiKey
    {
        Key
    }
}
