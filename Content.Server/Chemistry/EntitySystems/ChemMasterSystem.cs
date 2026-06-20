using Content.Server.Chemistry.Components;
using Content.Server.Popups;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Paper;
using Content.Shared._Art.Chemistry;
using Content.Shared._Starlight.Plumbing.Components; // Starlight-edit: Plumbing valve
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Storage;
using Content.Shared.Tag; // Starlight-edit
using Content.Shared.Whitelist;
using JetBrains.Annotations;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Content.Server.Chemistry.EntitySystems
{
    /// <summary>
    /// Contains all the server-side logic for ChemMasters.
    /// <seealso cref="ChemMasterComponent"/>
    /// </summary>
    [UsedImplicitly]
    public sealed class ChemMasterSystem : EntitySystem
    {
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly AudioSystem _audioSystem = default!;
        [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly UserInterfaceSystem _userInterfaceSystem = default!;
        [Dependency] private readonly StorageSystem _storageSystem = default!;
        [Dependency] private readonly LabelSystem _labelSystem = default!;
        [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly SharedHandsSystem _hands = default!; // SS14-Art
        [Dependency] private readonly PaperSystem _paper = default!;       // SS14-Art
        [Dependency] private readonly MetaDataSystem _metaData = default!; // SS14-Art
        [Dependency] private readonly TagSystem _tag = default!; // Starlight-edit

        private static readonly EntProtoId PillPrototypeId = "Pill";
        private static readonly EntProtoId PatchPrototypeId = "Patch"; // Starlight

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ChemMasterComponent, ComponentStartup>(SubscribeUpdateUiState);
            SubscribeLocalEvent<ChemMasterComponent, SolutionChangedEvent>(SubscribeUpdateUiState);
            SubscribeLocalEvent<ChemMasterComponent, BoundUIOpenedEvent>(SubscribeUpdateUiState);
            SubscribeLocalEvent<ChemMasterComponent, EntInsertedIntoContainerMessage>(OnContainerInserted);
            SubscribeLocalEvent<ChemMasterComponent, EntRemovedFromContainerMessage>(OnContainerRemoved);
            SubscribeLocalEvent<ChemMasterComponent, MapInitEvent>(OnMapInit);

            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSetModeMessage>(OnSetModeMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSetPillTypeMessage>(OnSetPillTypeMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterReagentAmountButtonMessage>(OnReagentButtonMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterCreatePillsMessage>(OnCreatePillsMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterCreatePatchesMessage>(OnCreatePatchesMessage); // Starlight
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterOutputToBottleMessage>(OnOutputToBottleMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterToggleValveMessage>(OnToggleValveMessage); // Starlight

            // SS14-Art start: Chemical Analysis
            SubscribeLocalEvent<ChemMasterComponent, ChemAnalyzeReagentMessage>(OnAnalyzeReagentMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemPrintAnalysisMessage>(OnPrintAnalysisMessage);
            // SS14-Art end

            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSortMethodUpdated>(OnSortMethodUpdated);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterTransferringAmountUpdated>(OnTransferringAmountUpdated);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterAmountsUpdated>(OnAmountsUpdated);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterChooseReagentMessage>(OnChooseReagentMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterClearReagentSelectionMessage>(OnClearReagentSelectionMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterToggleBottleFillMessage>(OnToggleBottleFillMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterRowEjectMessage>(OnRowEjectMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSelectPillContainerSlotMessage>(OnSelectPillContainerSlotMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterTogglePillContainerFillMessage>(OnTogglePillContainerFillMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterPillContainerSlotEjectMessage>(OnPillContainerSlotEjectMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterPillContainerRowEjectMessage>(OnPillContainerRowEjectMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSelectPillCanisterForCreationMessage>(OnSelectPillCanisterForCreationMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSelectReagentAmountMessage>(OnSelectReagentAmountMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterRemoveReagentAmountMessage>(OnRemoveReagentAmountMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterClearReagentAmountMessage>(OnClearReagentAmountMessage);
            SubscribeLocalEvent<ChemMasterComponent, ItemSlotButtonPressedEvent>(OnItemSlotButtonPressed);
        }

        private void OnMapInit(EntityUid uid, ChemMasterComponent component, MapInitEvent args)
        {
            for (int i = 0; i < 3; i++)
            {
                var slotId = "pillContainerSlot" + i;
                ItemSlot slot = new();
                var whitelist = new EntityWhitelist();
                whitelist.Tags = new List<ProtoId<TagPrototype>> { "PillCanister" };
                slot.Whitelist = whitelist;
                _itemSlotsSystem.AddItemSlot(uid, slotId, slot);
            }

            for (int i = 0; i < 20; i++)
            {
                var slotId = "bottleSlot" + i;
                ItemSlot slot = new();
                var whitelist = new EntityWhitelist();
                whitelist.Tags = new List<ProtoId<TagPrototype>> { "Bottle" };
                slot.Whitelist = whitelist;
                _itemSlotsSystem.AddItemSlot(uid, slotId, slot);
            }
        }

        private void SubscribeUpdateUiState<T>(Entity<ChemMasterComponent> ent, ref T ev)
        {
            UpdateUiState(ent);
        }

        private void UpdateUiState(Entity<ChemMasterComponent> ent, bool updateLabel = false)
        {
            var (owner, chemMaster) = ent;

            if (!_solutionContainerSystem.TryGetSolution(owner, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                return;

            // Clamp selected amounts to what's actually in buffer
            var reagentsToRemove = new List<ReagentId>();
            var reagentsToUpdate = new Dictionary<ReagentId, float>();

            foreach (var (reagentId, selectedAmount) in chemMaster.SelectedReagentAmounts)
            {
                var availableAmount = bufferSolution.GetReagentQuantity(reagentId);
                if (availableAmount <= 0)
                    reagentsToRemove.Add(reagentId);
                else if (selectedAmount > (float)availableAmount)
                    reagentsToUpdate[reagentId] = (float)availableAmount;
            }
            foreach (var r in reagentsToRemove)
                chemMaster.SelectedReagentAmounts.Remove(r);
            foreach (var (r, v) in reagentsToUpdate)
                chemMaster.SelectedReagentAmounts[r] = v;

            // Reset bottle selection if slot is empty
            if (chemMaster.SelectedBottleForFill >= 0 && chemMaster.SelectedBottleForFill < 20)
            {
                var slotId = "bottleSlot" + chemMaster.SelectedBottleForFill;
                if (!_itemSlotsSystem.TryGetSlot(owner, slotId, out var bottleSlot) || !bottleSlot.Item.HasValue)
                    chemMaster.SelectedBottleForFill = -1;
            }

            // Ensure list sizes
            if (chemMaster.StoredPillContainers.Count != 3)
            {
                chemMaster.StoredPillContainers.Clear();
                for (int i = 0; i < 3; i++)
                    chemMaster.StoredPillContainers.Add(null);
            }
            if (chemMaster.StoredBottles.Count != 20)
            {
                chemMaster.StoredBottles.Clear();
                for (int i = 0; i < 20; i++)
                    chemMaster.StoredBottles.Add(null);
            }

            var container = _itemSlotsSystem.GetItemOrNull(owner, SharedChemMaster.InputSlotName);
            var bufferReagents = bufferSolution.Contents;
            var bufferCurrentVolume = bufferSolution.Volume;

            // Build pill container data
            var storedPillContainersInfo = new List<ContainerInfo?>();
            var pillContainers = new List<List<bool>>();
            var pillTypes = new List<List<uint>>();
            chemMaster.StoredPillContainers.Clear();

            for (int i = 0; i < 3; i++)
            {
                var slotId = "pillContainerSlot" + i;
                if (_itemSlotsSystem.TryGetSlot(owner, slotId, out var slot) && slot.Item.HasValue)
                {
                    var pillContainer = slot.Item.Value;
                    if (TryComp(pillContainer, out StorageComponent? storage))
                    {
                        var pillCount = storage.Container.ContainedEntities.Count;
                        const int maxPills = 10;
                        var slotStates = new List<bool>();
                        var slotPillTypes = new List<uint>();

                        for (int j = 0; j < maxPills; j++)
                        {
                            if (j < pillCount)
                            {
                                slotStates.Add(true);
                                var pillEntity = storage.Container.ContainedEntities.ElementAt(j);
                                slotPillTypes.Add(TryComp<PillComponent>(pillEntity, out var pc) ? pc.PillType : 0u);
                            }
                            else
                            {
                                slotStates.Add(false);
                                slotPillTypes.Add(0);
                            }
                        }
                        pillContainers.Add(slotStates);
                        pillTypes.Add(slotPillTypes);
                        storedPillContainersInfo.Add(new ContainerInfo(Name(pillContainer), pillCount, maxPills));
                    }
                    else
                    {
                        pillContainers.Add(Enumerable.Repeat(false, 10).ToList());
                        pillTypes.Add(Enumerable.Repeat(0u, 10).ToList());
                        storedPillContainersInfo.Add(new ContainerInfo(Name(pillContainer), 0, 10));
                    }
                    chemMaster.StoredPillContainers.Add(pillContainer);
                }
                else
                {
                    pillContainers.Add(Enumerable.Repeat(false, 10).ToList());
                    pillTypes.Add(Enumerable.Repeat(0u, 10).ToList());
                    storedPillContainersInfo.Add(null);
                    chemMaster.StoredPillContainers.Add(null);
                }
            }

            // Build bottle data
            var storedBottlesInfo = new List<ContainerInfo?>();
            chemMaster.StoredBottles.Clear();

            for (int i = 0; i < 20; i++)
            {
                var slotId = "bottleSlot" + i;
                if (_itemSlotsSystem.TryGetSlot(owner, slotId, out var slot) && slot.Item.HasValue)
                {
                    var bottle = slot.Item.Value;
                    if (_solutionContainerSystem.TryGetSolution(bottle, SharedChemMaster.BottleSolutionName, out _, out var sol))
                        storedBottlesInfo.Add(BuildContainerInfo(Name(bottle), sol));
                    else
                        storedBottlesInfo.Add(new ContainerInfo(Name(bottle), 0, FixedPoint2.New(50)));
                    chemMaster.StoredBottles.Add(bottle);
                }
                else
                {
                    storedBottlesInfo.Add(null);
                    chemMaster.StoredBottles.Add(null);
                }
            }

            // Selected pill container info
            ContainerInfo? selectedPillContainerInfo = null;
            if (chemMaster.SelectedPillContainerSlot >= 0 && chemMaster.SelectedPillContainerSlot < 30)
            {
                var containerIndex = chemMaster.SelectedPillContainerSlot / 10;
                var slotIndex = chemMaster.SelectedPillContainerSlot % 10;

                if (containerIndex < chemMaster.StoredPillContainers.Count &&
                    chemMaster.StoredPillContainers[containerIndex] != null &&
                    containerIndex < pillContainers.Count &&
                    slotIndex < pillContainers[containerIndex].Count &&
                    pillContainers[containerIndex][slotIndex])
                {
                    var pillContainer = chemMaster.StoredPillContainers[containerIndex];
                    if (pillContainer != null && TryComp(pillContainer.Value, out StorageComponent? st))
                    {
                        if (slotIndex < st.Container.ContainedEntities.Count)
                        {
                            var pillEntity = st.Container.ContainedEntities.ElementAt(slotIndex);
                            if (_solutionContainerSystem.TryGetSolution(pillEntity, SharedChemMaster.PillSolutionName, out _, out var pillSolution))
                                selectedPillContainerInfo = BuildContainerInfo(Name(pillEntity), pillSolution);
                        }
                    }
                }
            }

            // Starlight: valve
            var valveOpen = TryComp<PlumbingOutletComponent>(owner, out var plumbingOutlet) && plumbingOutlet.Enabled;

            var state = new ChemMasterBoundUserInterfaceState(
                chemMaster.Mode,
                BuildInputContainerInfo(container),
                bufferReagents,
                bufferCurrentVolume,
                chemMaster.PillType,
                chemMaster.PillDosageLimit,
                chemMaster.PatchDosageLimit,
                chemMaster.BottleDosageLimit,
                chemMaster.MaxPills,
                chemMaster.MaxBottles,
                updateLabel,
                chemMaster.SortMethod,
                chemMaster.TransferringAmount,
                chemMaster.Amounts,
                storedPillContainersInfo,
                pillContainers,
                pillTypes,
                chemMaster.SelectedPillContainerSlot,
                chemMaster.SelectedPillContainerForFill,
                chemMaster.SelectedPillCanisterForCreation,
                chemMaster.SelectedReagent,
                selectedPillContainerInfo,
                storedBottlesInfo,
                chemMaster.SelectedBottleForFill,
                chemMaster.SelectedReagents,
                chemMaster.SelectedReagentAmounts,
                valveOpen);

            _userInterfaceSystem.SetUiState(owner, ChemMasterUiKey.Key, state);
        }

        private void OnSetModeMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSetModeMessage message)
        {
            if (!Enum.IsDefined(typeof(ChemMasterMode), message.ChemMasterMode))
                return;
            chemMaster.Comp.Mode = message.ChemMasterMode;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnSetPillTypeMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSetPillTypeMessage message)
        {
            if (message.PillType > SharedChemMaster.PillTypes - 1)
                return;
            chemMaster.Comp.PillType = message.PillType;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnReagentButtonMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterReagentAmountButtonMessage message)
        {
            switch (chemMaster.Comp.Mode)
            {
                case ChemMasterMode.Transfer:
                    TransferReagents(chemMaster, message.ReagentId, message.Amount, message.FromBuffer, message.IsOutput);
                    break;
                case ChemMasterMode.Discard:
                    DiscardReagents(chemMaster, message.ReagentId, message.Amount, message.FromBuffer, message.IsOutput);
                    break;
                default:
                    return;
            }
            ClickSound(chemMaster);
        }

        private void OnSortMethodUpdated(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSortMethodUpdated args)
        {
            chemMaster.Comp.SortMethod = args.SortMethod;
            UpdateUiState(chemMaster);
        }

        private void OnTransferringAmountUpdated(Entity<ChemMasterComponent> chemMaster, ref ChemMasterTransferringAmountUpdated args)
        {
            chemMaster.Comp.TransferringAmount = args.TransferringAmount;
            ClickSound(chemMaster);
            UpdateUiState(chemMaster);
        }

        private void OnAmountsUpdated(Entity<ChemMasterComponent> chemMaster, ref ChemMasterAmountsUpdated args)
        {
            chemMaster.Comp.Amounts = args.Amounts;
            UpdateUiState(chemMaster);
        }

        private void OnChooseReagentMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterChooseReagentMessage message)
        {
            chemMaster.Comp.SelectedReagent = message.Reagent;
            UpdateUiState(chemMaster);
        }

        private void OnClearReagentSelectionMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterClearReagentSelectionMessage message)
        {
            chemMaster.Comp.SelectedReagent = null;
            UpdateUiState(chemMaster);
        }

        private void OnToggleBottleFillMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterToggleBottleFillMessage message)
        {
            if (message.Slot < 0 || message.Slot >= chemMaster.Comp.StoredBottles.Count || chemMaster.Comp.StoredBottles[message.Slot] == null)
                return;
            chemMaster.Comp.SelectedBottleForFill = chemMaster.Comp.SelectedBottleForFill == message.Slot ? -1 : message.Slot;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnRowEjectMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterRowEjectMessage message)
        {
            var startSlot = message.Row * 4;
            var endSlot = startSlot + 3;
            for (int slot = startSlot; slot <= endSlot; slot++)
            {
                if (slot >= chemMaster.Comp.StoredBottles.Count) continue;
                var slotName = $"bottleSlot{slot}";
                if (_itemSlotsSystem.TryGetSlot((EntityUid)chemMaster, slotName, out var itemSlot))
                    _itemSlotsSystem.TryEject((EntityUid)chemMaster, itemSlot, message.Actor, out _, excludeUserAudio: true);
            }
            UpdateUiState(chemMaster);
        }

        private void OnSelectPillContainerSlotMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSelectPillContainerSlotMessage message)
        {
            if (message.Slot < 0 || message.Slot >= 30) return;
            chemMaster.Comp.SelectedPillContainerSlot = chemMaster.Comp.SelectedPillContainerSlot == message.Slot ? -1 : message.Slot;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnTogglePillContainerFillMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterTogglePillContainerFillMessage message)
        {
            if (message.Slot < 0 || message.Slot >= 30) return;
            chemMaster.Comp.SelectedPillContainerForFill = chemMaster.Comp.SelectedPillContainerForFill == message.Slot ? -1 : message.Slot;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnPillContainerSlotEjectMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterPillContainerSlotEjectMessage message)
        {
            if (message.Slot < 0 || message.Slot >= 30) return;
            var containerIndex = message.Slot / 10;
            var slotIndex = message.Slot % 10;

            if (containerIndex < chemMaster.Comp.StoredPillContainers.Count &&
                chemMaster.Comp.StoredPillContainers[containerIndex] is { } pillContainer &&
                TryComp(pillContainer, out StorageComponent? storage) &&
                storage.Container.ContainedEntities.Count > slotIndex)
            {
                var pillToEject = storage.Container.ContainedEntities.ElementAt(slotIndex);
                _storageSystem.Insert(pillContainer, pillToEject, out _, user: chemMaster);
            }
            UpdateUiState(chemMaster);
        }

        private void OnPillContainerRowEjectMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterPillContainerRowEjectMessage message)
        {
            if (message.Row < 0 || message.Row >= 3) return;
            var startSlot = message.Row * 10;
            for (int slot = startSlot; slot < startSlot + 10; slot++)
            {
                var containerIndex = slot / 10;
                var slotIndex = slot % 10;
                if (containerIndex < chemMaster.Comp.StoredPillContainers.Count &&
                    chemMaster.Comp.StoredPillContainers[containerIndex] is { } pillContainer &&
                    TryComp(pillContainer, out StorageComponent? storage) &&
                    storage.Container.ContainedEntities.Count > slotIndex)
                {
                    var pillToEject = storage.Container.ContainedEntities.ElementAt(slotIndex);
                    _storageSystem.Insert(pillContainer, pillToEject, out _, user: chemMaster);
                }
            }
            UpdateUiState(chemMaster);
        }

        private void OnSelectPillCanisterForCreationMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSelectPillCanisterForCreationMessage message)
        {
            if (message.CanisterIndex < 0 || message.CanisterIndex >= 3) return;
            chemMaster.Comp.SelectedPillCanisterForCreation = chemMaster.Comp.SelectedPillCanisterForCreation == message.CanisterIndex ? -1 : message.CanisterIndex;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnItemSlotButtonPressed(Entity<ChemMasterComponent> chemMaster, ref ItemSlotButtonPressedEvent message)
        {
            if (message.SlotId.StartsWith("pillContainerSlot") &&
                int.TryParse(message.SlotId.Replace("pillContainerSlot", ""), out int canisterIndex) &&
                canisterIndex >= 0 && canisterIndex < 3)
            {
                var slotId = $"pillContainerSlot{canisterIndex}";
                if (_itemSlotsSystem.TryGetSlot(chemMaster, slotId, out var slot) && slot.Item.HasValue)
                    _itemSlotsSystem.TryEject((EntityUid)chemMaster, slot, message.Actor, out _, excludeUserAudio: true);
                UpdateUiState(chemMaster);
            }
        }

        private void OnSelectReagentAmountMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSelectReagentAmountMessage message)
        {
            if (message.Amount <= 0) return;
            if (!_solutionContainerSystem.TryGetSolution((EntityUid)chemMaster, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                return;

            var availableInBuffer = bufferSolution.GetReagentQuantity(message.Reagent);

            // Try direct lookup first, then by prototype
            if (chemMaster.Comp.SelectedReagentAmounts.TryGetValue(message.Reagent, out var currentAmount))
            {
                chemMaster.Comp.SelectedReagentAmounts[message.Reagent] = Math.Min(currentAmount + message.Amount, (float)availableInBuffer);
            }
            else
            {
                ReagentId? existingKey = null;
                float existingAmount = 0;
                foreach (var kvp in chemMaster.Comp.SelectedReagentAmounts)
                {
                    if (kvp.Key.Prototype == message.Reagent.Prototype)
                    {
                        existingKey = kvp.Key;
                        existingAmount = kvp.Value;
                        break;
                    }
                }

                if (existingKey != null)
                    chemMaster.Comp.SelectedReagentAmounts[existingKey.Value] = Math.Min(existingAmount + message.Amount, (float)availableInBuffer);
                else
                    chemMaster.Comp.SelectedReagentAmounts[message.Reagent] = Math.Min(message.Amount, (float)availableInBuffer);
            }

            CorrectSelectedReagentAmounts(chemMaster.Comp, bufferSolution);
            UpdateUiState(chemMaster, updateLabel: true);
            ClickSound(chemMaster);
        }

        private void OnRemoveReagentAmountMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterRemoveReagentAmountMessage message)
        {
            if (message.Amount <= 0) return;

            if (chemMaster.Comp.SelectedReagentAmounts.TryGetValue(message.Reagent, out var currentAmount))
            {
                var newAmount = Math.Max(0, currentAmount - message.Amount);
                if (newAmount == 0)
                    chemMaster.Comp.SelectedReagentAmounts.Remove(message.Reagent);
                else
                    chemMaster.Comp.SelectedReagentAmounts[message.Reagent] = newAmount;
            }
            else
            {
                ReagentId? existingKey = null;
                float existingAmount = 0;
                foreach (var kvp in chemMaster.Comp.SelectedReagentAmounts)
                {
                    if (kvp.Key.Prototype == message.Reagent.Prototype)
                    {
                        existingKey = kvp.Key;
                        existingAmount = kvp.Value;
                        break;
                    }
                }
                if (existingKey != null)
                {
                    var newAmount = Math.Max(0, existingAmount - message.Amount);
                    if (newAmount == 0)
                        chemMaster.Comp.SelectedReagentAmounts.Remove(existingKey.Value);
                    else
                        chemMaster.Comp.SelectedReagentAmounts[existingKey.Value] = newAmount;
                }
            }

            UpdateUiState(chemMaster, updateLabel: true);
            ClickSound(chemMaster);
        }

        private void OnClearReagentAmountMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterClearReagentAmountMessage message)
        {
            if (!chemMaster.Comp.SelectedReagentAmounts.Remove(message.Reagent))
            {
                ReagentId? existingKey = null;
                foreach (var kvp in chemMaster.Comp.SelectedReagentAmounts)
                {
                    if (kvp.Key.Prototype == message.Reagent.Prototype) { existingKey = kvp.Key; break; }
                }
                if (existingKey != null)
                    chemMaster.Comp.SelectedReagentAmounts.Remove(existingKey.Value);
            }
            UpdateUiState(chemMaster, updateLabel: true);
            ClickSound(chemMaster);
        }

        private void OnContainerInserted(Entity<ChemMasterComponent> chemMaster, ref EntInsertedIntoContainerMessage args)
        {
            if (args.Container?.ID?.StartsWith("pillContainerSlot") == true)
            {
                UpdateUiState(chemMaster);
                return;
            }
            if (IsPillContainerStorage(chemMaster, args.Container?.Owner))
            {
                UpdateUiState(chemMaster);
                return;
            }

            if (args.Container?.ID == SharedChemMaster.OutputSlotName)
            {
                var entity = args.Entity;
                if (_solutionContainerSystem.TryGetSolution(entity, SharedChemMaster.BottleSolutionName, out _, out _))
                {
                    var owner = chemMaster;
                    _itemSlotsSystem.TryEject(owner, SharedChemMaster.OutputSlotName, null, out var ejected, excludeUserAudio: true);
                    var moving = ejected ?? entity;

                    for (int row = 0; row < 4; row++)
                    {
                        for (int col = 0; col < 5; col++)
                        {
                            var i = row * 5 + col;
                            var slotId = "bottleSlot" + i;
                            if (_itemSlotsSystem.TryGetSlot(owner, slotId, out var slot) && !slot.HasItem)
                            {
                                if (_itemSlotsSystem.TryInsert(owner, slotId, moving, null))
                                {
                                    UpdateUiState(chemMaster);
                                    ClickSound(chemMaster);
                                    return;
                                }
                            }
                        }
                    }
                }
                UpdateUiState(chemMaster);
                return;
            }

            UpdateUiState(chemMaster);
        }

        private void OnContainerRemoved(Entity<ChemMasterComponent> chemMaster, ref EntRemovedFromContainerMessage args)
        {
            if (IsPillContainerStorage(chemMaster, args.Container?.Owner))
            {
                UpdateUiState(chemMaster);
                return;
            }
            UpdateUiState(chemMaster);
        }

        private bool IsPillContainerStorage(Entity<ChemMasterComponent> chemMaster, EntityUid? storageEntity)
        {
            if (storageEntity == null) return false;
            foreach (var stored in chemMaster.Comp.StoredPillContainers)
            {
                if (stored == storageEntity) return true;
            }
            return false;
        }

        private (int totalAvailableSlots, List<(EntityUid container, int startSlot)> availableSlots) FindAvailablePillSlotsForCreation(Entity<ChemMasterComponent> chemMaster, int requestedPills)
        {
            var availableSlots = new List<(EntityUid, int)>();
            var totalAvailableSlots = 0;

            if (chemMaster.Comp.SelectedPillCanisterForCreation >= 0 && chemMaster.Comp.SelectedPillCanisterForCreation < 3)
            {
                var slotId = "pillContainerSlot" + chemMaster.Comp.SelectedPillCanisterForCreation;
                if (_itemSlotsSystem.TryGetSlot(chemMaster, slotId, out var slot) && slot.Item.HasValue &&
                    TryComp(slot.Item.Value, out StorageComponent? storage))
                {
                    var empty = 10 - storage.Container.ContainedEntities.Count;
                    totalAvailableSlots = Math.Min(empty, requestedPills);
                    if (totalAvailableSlots > 0)
                        availableSlots.Add((slot.Item.Value, storage.Container.ContainedEntities.Count));
                }
            }
            else
            {
                for (int i = 0; i < 3 && totalAvailableSlots < requestedPills; i++)
                {
                    var slotId = "pillContainerSlot" + i;
                    if (_itemSlotsSystem.TryGetSlot(chemMaster, slotId, out var slot) && slot.Item.HasValue &&
                        TryComp(slot.Item.Value, out StorageComponent? storage))
                    {
                        var empty = 10 - storage.Container.ContainedEntities.Count;
                        var toUse = Math.Min(empty, requestedPills - totalAvailableSlots);
                        if (toUse > 0)
                        {
                            availableSlots.Add((slot.Item.Value, storage.Container.ContainedEntities.Count));
                            totalAvailableSlots += toUse;
                        }
                    }
                }
            }
            return (totalAvailableSlots, availableSlots);
        }

        private void TransferReagents(Entity<ChemMasterComponent> chemMaster, ReagentId id, FixedPoint2 amount, bool fromBuffer, bool isOutput)
        {
            Entity<SolutionComponent>? containerSoln;
            Solution? containerSolution;

            if (chemMaster.Comp.SelectedBottleForFill >= 0 &&
                chemMaster.Comp.StoredBottles[chemMaster.Comp.SelectedBottleForFill] is { } fillBottle &&
                fromBuffer && isOutput)
            {
                if (!_solutionContainerSystem.TryGetSolution(fillBottle, SharedChemMaster.BottleSolutionName, out containerSoln, out containerSolution))
                    return;
            }
            else if (!fromBuffer && !isOutput)
            {
                var inputContainer = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.InputSlotName);
                if (inputContainer != null &&
                    _solutionContainerSystem.TryGetFitsInDispenser(inputContainer.Value, out var inputSoln, out var inputSol) &&
                    inputSol.GetReagentQuantity(id) > 0)
                {
                    containerSoln = inputSoln;
                    containerSolution = inputSol;
                }
                else if (chemMaster.Comp.SelectedBottleForFill >= 0 &&
                    chemMaster.Comp.StoredBottles[chemMaster.Comp.SelectedBottleForFill] is { } viewBottle &&
                    _solutionContainerSystem.TryGetSolution(viewBottle, SharedChemMaster.BottleSolutionName, out containerSoln, out containerSolution))
                {
                    // use bottle
                }
                else
                {
                    var fallback = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.InputSlotName);
                    if (fallback is null || !_solutionContainerSystem.TryGetFitsInDispenser(fallback.Value, out containerSoln, out containerSolution))
                        return;
                }
            }
            else if (!fromBuffer)
            {
                var inputContainer = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.InputSlotName);
                if (inputContainer is null || !_solutionContainerSystem.TryGetFitsInDispenser(inputContainer.Value, out containerSoln, out containerSolution))
                    return;
            }
            else if (fromBuffer && chemMaster.Comp.SelectedBottleForFill >= 0 &&
                     chemMaster.Comp.StoredBottles[chemMaster.Comp.SelectedBottleForFill] is { } slotBottle)
            {
                if (!_solutionContainerSystem.TryGetSolution(slotBottle, SharedChemMaster.BottleSolutionName, out containerSoln, out containerSolution))
                    return;
            }
            else
            {
                var inputContainer = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.InputSlotName);
                if (inputContainer is null || !_solutionContainerSystem.TryGetFitsInDispenser(inputContainer.Value, out containerSoln, out containerSolution))
                    return;
            }

            if (!_solutionContainerSystem.TryGetSolution((EntityUid)chemMaster, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                return;

            if (fromBuffer)
            {
                var available = bufferSolution.GetReagentQuantity(id);
                if (amount == int.MaxValue) amount = available;
                amount = FixedPoint2.Min(amount, available, containerSolution.AvailableVolume);
                amount = bufferSolution.RemoveReagent(id, amount, preserveOrder: true);
                _solutionContainerSystem.TryAddReagent(containerSoln!.Value, id, amount, out _);
            }
            else
            {
                var available = containerSolution.GetReagentQuantity(id);
                if (amount == int.MaxValue) amount = available;
                amount = FixedPoint2.Min(amount, available);
                if (bufferSolution.MaxVolume.Value > 0)
                    amount = FixedPoint2.Min(amount, bufferSolution.AvailableVolume);
                _solutionContainerSystem.RemoveReagent(containerSoln!.Value, id, amount);
                bufferSolution.AddReagent(id, amount);
            }

            UpdateUiState(chemMaster, updateLabel: true);
        }

        private void DiscardReagents(Entity<ChemMasterComponent> chemMaster, ReagentId id, FixedPoint2 amount, bool fromBuffer, bool isOutput)
        {
            if (fromBuffer)
            {
                if (!_solutionContainerSystem.TryGetSolution((EntityUid)chemMaster, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                    return;
                var available = bufferSolution.GetReagentQuantity(id);
                if (amount == int.MaxValue) amount = available;
                bufferSolution.RemoveReagent(id, FixedPoint2.Min(amount, available), preserveOrder: true);
            }
            else
            {
                Entity<SolutionComponent>? containerSoln;
                if (chemMaster.Comp.SelectedBottleForFill >= 0 &&
                    chemMaster.Comp.StoredBottles[chemMaster.Comp.SelectedBottleForFill] is { } bottle &&
                    _solutionContainerSystem.TryGetSolution(bottle, SharedChemMaster.BottleSolutionName, out containerSoln, out var bottleSol))
                {
                    var available = bottleSol.GetReagentQuantity(id);
                    if (amount == int.MaxValue) amount = available;
                    _solutionContainerSystem.RemoveReagent(containerSoln.Value, id, FixedPoint2.Min(amount, available));
                }
                else
                {
                    var inputContainer = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.InputSlotName);
                    if (inputContainer is null || !_solutionContainerSystem.TryGetFitsInDispenser(inputContainer.Value, out containerSoln, out var inputSol))
                        return;
                    var available = inputSol.GetReagentQuantity(id);
                    if (amount == int.MaxValue) amount = available;
                    _solutionContainerSystem.RemoveReagent(containerSoln.Value, id, FixedPoint2.Min(amount, available));
                }
            }
            UpdateUiState(chemMaster, updateLabel: fromBuffer);
        }

        private void OnCreatePillsMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterCreatePillsMessage message)
        {
            var user = message.Actor;

            if (message.Dosage == 0 || message.Dosage > chemMaster.Comp.PillDosageLimit) return;
            if (message.Label.Length > SharedChemMaster.LabelMaxLength) return;
            if (chemMaster.Comp.SelectedReagentAmounts.Count == 0) return;

            var totalVolume = FixedPoint2.Zero;
            foreach (var amount in chemMaster.Comp.SelectedReagentAmounts.Values)
                totalVolume += FixedPoint2.New(amount);

            var pillsToCreate = (int)message.Number;
            if (message.Dosage * message.Number > totalVolume)
                pillsToCreate = Math.Min((int)(totalVolume / message.Dosage), (int)message.Number);
            if (pillsToCreate == 0) return;

            var (totalAvailableSlots, availableSlots) = FindAvailablePillSlotsForCreation(chemMaster, pillsToCreate);
            var actualPillsToCreate = Math.Min(pillsToCreate, totalAvailableSlots);
            if (actualPillsToCreate == 0)
            {
                _popupSystem.PopupCursor(Loc.GetString("chem-master-pills-created", ("created", 0), ("requested", (int)message.Number)), user);
                return;
            }

            if (!_solutionContainerSystem.TryGetSolution((EntityUid)chemMaster, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                return;
            if (bufferSolution.Volume == 0) { _popupSystem.PopupCursor(Loc.GetString("chem-master-window-buffer-empty-text"), user); return; }

            var totalNeeded = FixedPoint2.New(actualPillsToCreate * message.Dosage);
            var totalSelectedAmount = FixedPoint2.Zero;
            foreach (var amount in chemMaster.Comp.SelectedReagentAmounts.Values)
                totalSelectedAmount += FixedPoint2.New(amount);
            if (totalSelectedAmount <= 0) return;

            var scale = totalNeeded / totalSelectedAmount;
            var proportionalAmounts = new Dictionary<ReagentId, FixedPoint2>();
            var totalForWithdrawal = FixedPoint2.Zero;
            var reagentList = chemMaster.Comp.SelectedReagentAmounts.ToList();

            for (int i = 0; i < reagentList.Count - 1; i++)
            {
                var (reagent, selectedAmount) = reagentList[i];
                var scaledAmount = FixedPoint2.New(selectedAmount) * scale;
                proportionalAmounts[reagent] = scaledAmount;
                totalForWithdrawal += scaledAmount;
            }
            if (reagentList.Count > 0)
            {
                var lastReagent = reagentList[reagentList.Count - 1].Key;
                proportionalAmounts[lastReagent] = totalNeeded - totalForWithdrawal;
            }

            foreach (var (reagent, neededAmount) in proportionalAmounts)
            {
                if (bufferSolution.GetReagentQuantity(reagent) < neededAmount)
                {
                    _popupSystem.PopupCursor(Loc.GetString("chem-master-window-buffer-low-text"), user);
                    return;
                }
            }

            var withdrawnAmountsDict = new Dictionary<ReagentId, float>();
            foreach (var (reagent, neededAmount) in proportionalAmounts)
            {
                var actualAmount = bufferSolution.RemoveReagent(reagent, neededAmount, preserveOrder: true);
                withdrawnAmountsDict[reagent] = (float)actualAmount;
            }

            UpdateSelectedReagentAmountsAfterWithdrawal(chemMaster, withdrawnAmountsDict, bufferSolution);

            var pillIndex = 0;
            foreach (var (pillContainer, startSlot) in availableSlots)
            {
                if (!TryComp(pillContainer, out StorageComponent? targetStorage)) continue;
                var slotsAvailable = 10 - targetStorage.Container.ContainedEntities.Count;
                var slotsToFill = Math.Min(slotsAvailable, actualPillsToCreate - pillIndex);

                for (int s = 0; s < slotsToFill && pillIndex < actualPillsToCreate; s++, pillIndex++)
                {
                    var item = Spawn(PillPrototypeId, Transform(pillContainer).Coordinates);
                    _solutionContainerSystem.EnsureSolutionEntity(item, SharedChemMaster.PillSolutionName, out var itemSolution, message.Dosage);
                    if (!itemSolution.HasValue) continue;

                    var pillSolution = new Solution();
                    foreach (var (reagent, perPillAmount) in proportionalAmounts)
                        pillSolution.AddReagent(reagent, perPillAmount / actualPillsToCreate);

                    _solutionContainerSystem.TryAddSolution(itemSolution.Value, pillSolution);
                    _labelSystem.Label(item, message.Label);

                    var pill = EnsureComp<PillComponent>(item);
                    pill.PillType = chemMaster.Comp.PillType;
                    Dirty(item, pill);

                    _storageSystem.Insert(pillContainer, item, out _, user: user, targetStorage);

                    _adminLogger.Add(LogType.Action, LogImpact.Low,
                        $"{ToPrettyString(user):user} printed {ToPrettyString(item):pill} {SharedSolutionContainerSystem.ToPrettyString(itemSolution.Value.Comp.Solution)}");
                }
            }

            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        // Starlight-start
        private void OnCreatePatchesMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterCreatePatchesMessage message)
        {
            var user = message.Actor;

            if (message.Dosage == 0 || message.Dosage > chemMaster.Comp.PatchDosageLimit) return;
            if (message.Label.Length > SharedChemMaster.LabelMaxLength) return;
            if (chemMaster.Comp.SelectedReagentAmounts.Count == 0) return;

            var totalVolume = FixedPoint2.Zero;
            foreach (var amount in chemMaster.Comp.SelectedReagentAmounts.Values)
                totalVolume += FixedPoint2.New(amount);

            var patchesToCreate = (int)message.Number;
            if (message.Dosage * message.Number > totalVolume)
                patchesToCreate = Math.Min((int)(totalVolume / message.Dosage), (int)message.Number);
            if (patchesToCreate == 0) return;

            if (!_solutionContainerSystem.TryGetSolution((EntityUid)chemMaster, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                return;

            var maybeContainer = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.OutputSlotName);
            if (maybeContainer is not { Valid: true } container || !TryComp(container, out StorageComponent? storage)) return;
            if (!_storageSystem.HasSpace((container, storage))) return;

            for (int i = 0; i < patchesToCreate; i++)
            {
                if (!_storageSystem.HasSpace((container, storage))) break;

                var item = Spawn(PatchPrototypeId, Transform(container).Coordinates);
                _storageSystem.Insert(container, item, out _, user: user, storage);
                _labelSystem.Label(item, message.Label);

                _solutionContainerSystem.EnsureSolutionEntity(item, SharedChemMaster.PatchSolutionName, out var itemSolution, message.Dosage);
                if (!itemSolution.HasValue) return;

                var patchSolution = new Solution();
                foreach (var (reagent, selectedAmount) in chemMaster.Comp.SelectedReagentAmounts)
                {
                    var perPatch = FixedPoint2.New(selectedAmount) / patchesToCreate;
                    var actual = bufferSolution.RemoveReagent(reagent, perPatch, preserveOrder: true);
                    patchSolution.AddReagent(reagent, actual);
                }
                _solutionContainerSystem.TryAddSolution(itemSolution.Value, patchSolution);
            }

            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }
        // Starlight-end

        private void OnOutputToBottleMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterOutputToBottleMessage message)
        {
            var user = message.Actor;

            if (message.Dosage == 0) return;
            if (message.Label.Length > SharedChemMaster.LabelMaxLength) return;
            if (chemMaster.Comp.SelectedReagentAmounts.Count == 0) return;

            if (!_solutionContainerSystem.TryGetSolution((EntityUid)chemMaster, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                return;

            var bottlesToFill = FindAvailableBottlesForFilling(chemMaster, (int)message.Number, message.Dosage);
            if (bottlesToFill.Count == 0) { _popupSystem.PopupCursor(Loc.GetString("chem-master-window-no-bottle-selected-text"), user); return; }

            var totalVolume = FixedPoint2.Zero;
            foreach (var amount in chemMaster.Comp.SelectedReagentAmounts.Values)
                totalVolume += FixedPoint2.New(amount);

            var bottlesToCreate = Math.Min((int)message.Number, bottlesToFill.Count);
            if (message.Dosage * bottlesToCreate > totalVolume)
                bottlesToCreate = Math.Min((int)(totalVolume / message.Dosage), bottlesToFill.Count);
            if (bottlesToCreate == 0) return;

            var totalNeeded = FixedPoint2.New(bottlesToCreate * message.Dosage);
            var totalSelectedAmount = FixedPoint2.Zero;
            foreach (var amount in chemMaster.Comp.SelectedReagentAmounts.Values)
                totalSelectedAmount += FixedPoint2.New(amount);
            if (totalSelectedAmount <= 0) return;

            var scale = totalNeeded / totalSelectedAmount;
            var proportionalAmounts = new Dictionary<ReagentId, FixedPoint2>();
            var totalForWithdrawal = FixedPoint2.Zero;
            var reagentList = chemMaster.Comp.SelectedReagentAmounts.ToList();

            for (int i = 0; i < reagentList.Count - 1; i++)
            {
                var (reagent, selectedAmount) = reagentList[i];
                var scaledAmount = FixedPoint2.New(selectedAmount) * scale;
                proportionalAmounts[reagent] = scaledAmount;
                totalForWithdrawal += scaledAmount;
            }
            if (reagentList.Count > 0)
            {
                var lastReagent = reagentList[reagentList.Count - 1].Key;
                proportionalAmounts[lastReagent] = totalNeeded - totalForWithdrawal;
            }

            var withdrawnAmountsDict = new Dictionary<ReagentId, float>();
            foreach (var (reagent, neededAmount) in proportionalAmounts)
            {
                var actual = bufferSolution.RemoveReagent(reagent, neededAmount, preserveOrder: true);
                withdrawnAmountsDict[reagent] = (float)actual;
            }
            UpdateSelectedReagentAmountsAfterWithdrawal(chemMaster, withdrawnAmountsDict, bufferSolution);

            for (int i = 0; i < bottlesToCreate; i++)
            {
                var bottle = bottlesToFill[i];
                if (!_solutionContainerSystem.TryGetSolution(bottle, SharedChemMaster.BottleSolutionName, out var soln, out _))
                    continue;

                var bottleSolution = new Solution();
                foreach (var (reagent, perBottleAmount) in proportionalAmounts)
                    bottleSolution.AddReagent(reagent, perBottleAmount / bottlesToCreate);

                _solutionContainerSystem.TryAddSolution(soln.Value, bottleSolution);
                _labelSystem.Label(bottle, message.Label);

                _adminLogger.Add(LogType.Action, LogImpact.Low,
                    $"{ToPrettyString(user):user} bottled {ToPrettyString(bottle):bottle}");
            }

            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private List<EntityUid> FindAvailableBottlesForFilling(Entity<ChemMasterComponent> chemMaster, int requestedBottles, uint dosage)
        {
            var targets = new List<EntityUid>(requestedBottles);
            var selected = chemMaster.Comp.SelectedBottleForFill;

            if (selected >= 0 && selected < chemMaster.Comp.StoredBottles.Count &&
                chemMaster.Comp.StoredBottles[selected] is { } selectedBottle &&
                _solutionContainerSystem.TryGetSolution(selectedBottle, SharedChemMaster.BottleSolutionName, out _, out var selSol) &&
                selSol.Volume == 0 && selSol.AvailableVolume >= dosage)
            {
                targets.Add(selectedBottle);
                return targets;
            }

            for (int i = 0; i < chemMaster.Comp.StoredBottles.Count && targets.Count < requestedBottles; i++)
            {
                var ent = chemMaster.Comp.StoredBottles[i];
                if (!ent.HasValue) continue;
                if (!_solutionContainerSystem.TryGetSolution(ent.Value, SharedChemMaster.BottleSolutionName, out var soln, out var solution)) continue;
                if (solution.Volume == 0 && solution.AvailableVolume >= dosage)
                    targets.Add(ent.Value);
            }
            return targets;
        }

        private void UpdateSelectedReagentAmountsAfterWithdrawal(Entity<ChemMasterComponent> chemMaster, Dictionary<ReagentId, float> withdrawnAmounts, Solution bufferSolution)
        {
            var updatedAmounts = new Dictionary<ReagentId, float>();
            foreach (var (reagent, selectedAmount) in chemMaster.Comp.SelectedReagentAmounts)
            {
                var withdrawn = withdrawnAmounts.GetValueOrDefault(reagent, 0);
                var remainingSelected = selectedAmount - withdrawn;
                var availableInBuffer = (float)bufferSolution.GetReagentQuantity(reagent);
                var newAmount = Math.Min(remainingSelected, availableInBuffer);
                if (newAmount > 0)
                    updatedAmounts[reagent] = newAmount;
            }
            chemMaster.Comp.SelectedReagentAmounts = updatedAmounts;
        }

        private void CorrectSelectedReagentAmounts(ChemMasterComponent chemMaster, Solution bufferSolution)
        {
            var updatedAmounts = new Dictionary<ReagentId, float>();
            foreach (var (reagent, selectedAmount) in chemMaster.SelectedReagentAmounts)
            {
                var availableInBuffer = (float)bufferSolution.GetReagentQuantity(reagent);
                if (selectedAmount > 0 && availableInBuffer > 0)
                    updatedAmounts[reagent] = Math.Min(selectedAmount, availableInBuffer);
            }
            chemMaster.SelectedReagentAmounts = updatedAmounts;
        }

        // SS14-Art start: Chemical Analysis
        private void OnAnalyzeReagentMessage(Entity<ChemMasterComponent> chemMaster, ref ChemAnalyzeReagentMessage message)
        {
            if (!_prototypeManager.TryIndex(message.ReagentId.Prototype, out ReagentPrototype? proto))
                return;

            _userInterfaceSystem.ServerSendUiMessage(
                chemMaster.Owner,
                ChemMasterUiKey.Key,
                new ChemReagentAnalysisPopupMessage(message.ReagentId, proto.LocalizedName, proto.LocalizedDescription),
                message.Actor);
        }

        private void OnPrintAnalysisMessage(Entity<ChemMasterComponent> chemMaster, ref ChemPrintAnalysisMessage message)
        {
            PrintAnalysis(message.Actor, message.ReagentId);
        }

        private void PrintAnalysis(EntityUid user, ReagentId reagentId)
        {
            if (!_prototypeManager.TryIndex(reagentId.Prototype, out ReagentPrototype? proto))
                return;

            var printed = Spawn("Paper", Transform(user).Coordinates);
            _hands.PickupOrDrop(user, printed, checkActionBlocker: false);

            if (!TryComp<PaperComponent>(printed, out var paperComp))
                return;

            _metaData.SetEntityName(printed, Loc.GetString("chem-analysis-paper-name", ("reagent", proto.LocalizedName)));

            var text =
                $"{Loc.GetString("chem-analysis-paper-title", ("reagent", proto.LocalizedName))}\n\n" +
                $"{Loc.GetString("chem-analysis-paper-name-line", ("reagent", proto.LocalizedName))}\n" +
                $"{Loc.GetString("chem-analysis-paper-description-line", ("description", proto.LocalizedDescription))}\n\n" +
                $"{Loc.GetString("chem-analysis-paper-notes-line")}";

            _paper.SetContent((printed, paperComp), text);
        }
        // SS14-Art end

        private void ClickSound(Entity<ChemMasterComponent> chemMaster)
        {
            _audioSystem.PlayPvs(chemMaster.Comp.ClickSound, chemMaster, AudioParams.Default.WithVolume(-2f));
        }

        private ContainerInfo? BuildInputContainerInfo(EntityUid? container)
        {
            if (container is not { Valid: true }) return null;
            if (!TryComp(container, out FitsInDispenserComponent? fits) ||
                !_solutionContainerSystem.TryGetSolution(container.Value, fits.Solution, out _, out var solution))
                return null;
            return BuildContainerInfo(Name(container.Value), solution);
        }

        private static ContainerInfo BuildContainerInfo(string name, Solution solution)
        {
            return new ContainerInfo(name, solution.Volume, solution.MaxVolume)
            {
                Reagents = solution.Contents
            };
        }

        // Starlight-start: Plumbing valve toggle
        private void OnToggleValveMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterToggleValveMessage message)
        {
            if (!TryComp<PlumbingOutletComponent>(chemMaster.Owner, out var plumbingOutlet))
                return;
            plumbingOutlet.Enabled = !plumbingOutlet.Enabled;
            Dirty(chemMaster.Owner, plumbingOutlet);
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }
        // Starlight-end
    }
}
