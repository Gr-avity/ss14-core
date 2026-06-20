using Content.Client._Art.Chemistry.UI;
using Content.Shared._Art.Chemistry;
using Content.Shared.Chemistry;
using Content.Shared.Containers.ItemSlots;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;

namespace Content.Client.Chemistry.UI
{
    /// <summary>
    /// Initializes a <see cref="ChemMasterWindow"/> and updates it when new server messages are received.
    /// </summary>
    [UsedImplicitly]
    public sealed class ChemMasterBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private ChemMasterWindow? _window;
        private ChemAnalysisPopup? _analysisPopup; // SS14-Art

        public ChemMasterBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            _window = this.CreateWindow<ChemMasterWindow>();
            _window.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;

            // Static button actions
            _window.InputEjectButton.OnPressed += _ => SendMessage(
                new ItemSlotButtonPressedEvent(SharedChemMaster.InputSlotName));
            _window.BufferTransferButton.OnPressed += _ => SendMessage(
                new ChemMasterSetModeMessage(ChemMasterMode.Transfer));
            _window.BufferDiscardButton.OnPressed += _ => SendMessage(
                new ChemMasterSetModeMessage(ChemMasterMode.Discard));
            _window.CreatePillButton.OnPressed += _ => HandleCreatePillPressed();
            _window.CreateBottleButton.OnPressed += _ => HandleCreateBottlePressed();

            // Starlight-start: Patch creation
            _window.CreatePatchButton.OnPressed += _ => HandleCreatePatchPressed();
            // Starlight-end

            // Starlight-start: Plumbing valve
            _window.OnToggleValveButtonPressed += () => SendMessage(new ChemMasterToggleValveMessage());
            // Starlight-end

            // SS14-Art start: Chemical Analysis
            _window.OnAnalyzeReagentPressed += reagent => SendMessage(new ChemAnalyzeReagentMessage(reagent));
            // SS14-Art end

            for (uint i = 0; i < _window.PillTypeButtons.Length; i++)
            {
                var pillType = i;
                _window.PillTypeButtons[i].OnPressed += _ => SendMessage(new ChemMasterSetPillTypeMessage(pillType));
            }

            // Transfer / amount / sorting events
            _window.OnReagentButtonPressed += (_, button, amount, isOutput) =>
                SendMessage(new ChemMasterReagentAmountButtonMessage(button.Id, amount, button.IsBuffer, isOutput));
            _window.OnSortMethodChanged += sortMethod =>
                SendMessage(new ChemMasterSortMethodUpdated(sortMethod));
            _window.OnTransferAmountChanged += amount =>
                SendMessage(new ChemMasterTransferringAmountUpdated(amount));
            _window.OnUpdateAmounts += amounts =>
                SendMessage(new ChemMasterAmountsUpdated(amounts));
            _window.OnTransferAllPressed += (reagent, isBuffer, isOutput) =>
                SendMessage(new ChemMasterReagentAmountButtonMessage(reagent, int.MaxValue, isBuffer, isOutput));

            // Bottle storage events
            _window.OnToggleBottleFillPressed += slot =>
                SendMessage(new ChemMasterToggleBottleFillMessage(slot));
            _window.OnBottleSlotEjectPressed += slot =>
                SendMessage(new ItemSlotButtonPressedEvent($"bottleSlot{slot}"));
            _window.OnRowEjectPressed += row =>
                SendMessage(new ChemMasterRowEjectMessage(row));

            // Pill container events
            _window.OnPillContainerSlotSelected += slot =>
                SendMessage(new ChemMasterSelectPillContainerSlotMessage(slot));
            _window.OnPillCanisterSelected += canisterIndex =>
                SendMessage(new ChemMasterSelectPillCanisterForCreationMessage(canisterIndex));
            _window.OnPillCanisterEjected += canisterIndex =>
                SendMessage(new ItemSlotButtonPressedEvent($"pillContainerSlot{canisterIndex}"));

            // Reagent amount selection events
            _window.OnSelectReagentAmount += (reagent, amount) =>
                SendMessage(new ChemMasterSelectReagentAmountMessage(reagent, amount));
            _window.OnRemoveReagentAmount += (reagent, amount) =>
                SendMessage(new ChemMasterRemoveReagentAmountMessage(reagent, amount));
            _window.OnTransferReagentFromBottle += (reagent, amount) =>
                SendMessage(new ChemMasterReagentAmountButtonMessage(reagent, amount, false, false));
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            var castState = (ChemMasterBoundUserInterfaceState) state;
            _window?.UpdateState(castState);
        }

        private void HandleCreatePillPressed()
        {
            if (_window == null) return;
            var pillLabel = _window.GeneratePillLabel();
            SendMessage(new ChemMasterCreatePillsMessage(
                (uint) _window.PillDosage.Value,
                (uint) _window.PillNumber.Value,
                pillLabel));
        }

        // Starlight-start
        private void HandleCreatePatchPressed()
        {
            if (_window == null) return;
            var label = _window.GeneratePatchLabel();
            SendMessage(new ChemMasterCreatePatchesMessage(
                (uint) _window.PatchDosage.Value,
                (uint) _window.PatchNumber.Value,
                label));
        }
        // Starlight-end

        private void HandleCreateBottlePressed()
        {
            if (_window == null) return;
            var bottleLabel = _window.GenerateBottleLabel();
            SendMessage(new ChemMasterOutputToBottleMessage(
                (uint) _window.BottleDosage.Value,
                (uint) _window.BottleNumber.Value,
                bottleLabel));
        }

        // SS14-Art start: Chemical Analysis popup
        protected override void ReceiveMessage(BoundUserInterfaceMessage message)
        {
            base.ReceiveMessage(message);

            if (message is not ChemReagentAnalysisPopupMessage popup)
                return;

            _analysisPopup?.Close();
            _analysisPopup = new ChemAnalysisPopup(popup);
            _analysisPopup.OnPrintPressed += reagent => SendMessage(new ChemPrintAnalysisMessage(reagent));
            _analysisPopup.OpenCentered();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
 
            if (disposing)
            {
                _analysisPopup?.Close();
                _analysisPopup = null;
            }
        }
        // SS14-Art end
    }
}
