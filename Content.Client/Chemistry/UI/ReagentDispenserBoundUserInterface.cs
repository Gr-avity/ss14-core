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
    /// Initializes a <see cref="ReagentDispenserWindow"/> and updates it when new server messages are received.
    /// </summary>
    [UsedImplicitly]
    public sealed class ReagentDispenserBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private ReagentDispenserWindow? _window;
        private ChemAnalysisPopup? _analysisPopup;

        public ReagentDispenserBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            _window = this.CreateWindow<ReagentDispenserWindow>();
            _window.Title = EntMan.GetComponent<MetaDataComponent>(Owner).EntityName;

            _window.EjectButton.OnPressed += _ => SendMessage(new ItemSlotButtonPressedEvent(SharedReagentDispenser.OutputSlotName));
            _window.AmountGrid.OnButtonPressed += s => SendMessage(new ReagentDispenserSetDispenseAmountMessage(s));
            _window.OnDispenseReagentButtonPressed += reagent => SendMessage(new ReagentDispenserDispenseReagentMessage(reagent));
            _window.OnContainerActionPressed += (reagent, action, quantity) =>
                SendMessage(new ReagentDispenserContainerActionMessage(reagent, action, quantity));

            // Starlight-start: Plumbing valve
            _window.OnToggleValveButtonPressed += () => SendMessage(new ReagentDispenserToggleValveMessage());
            // Starlight-end
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            var castState = (ReagentDispenserBoundUserInterfaceState) state;
            _window?.UpdateState(castState);
        }

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
    }
}
