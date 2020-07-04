using System;
using System.Windows.Input;
using Xamarin.Forms;


namespace PaperX_SCG_forms.Views.CustomControls
{
    public class FocusArgs
    {
        public bool CouldFocusBeSet { get; set; }
    }

    public class ScanBarcodeEntry : Entry
    {
        public static BindableProperty OnEnterPressedCommandProperty = BindableProperty.Create<ScanBarcodeEntry, ICommand>(x => x.OnEnterPressedCommand, null);
        public ICommand OnEnterPressedCommand
        {
            get { return (ICommand)this.GetValue(OnEnterPressedCommandProperty); }
            set { this.SetValue(OnEnterPressedCommandProperty, value); }
        }

        
        public event Action OnHideKeyboard_TriggerRenderer;

        public ScanBarcodeEntry() : base()
        {
            this.Focused += (sender, e) =>
            {
                if (!CanShowVirtualKeyboard)
                    HideKeyboard();
            };
        }

        public void HideKeyboard()
        {
            if (OnHideKeyboard_TriggerRenderer != null)
                OnHideKeyboard_TriggerRenderer();
        }


        public event EventHandler OnEnterPressed;
        public void EnterPressed()
        {
            if (OnEnterPressedCommand != null)
                this.OnEnterPressedCommand.Execute(this.OnEnterPressedCommandParameter);

            if (OnEnterPressed != null)
                OnEnterPressed(this, EventArgs.Empty);
        }


        public event EventHandler OnFocusChanged;

        private void FocusChanged()
        {
            if (OnFocusChanged != null)
                OnFocusChanged(this, EventArgs.Empty);
        }

        public const string CanShowVirtualKeyboardPropertyName = "CanShowVirtualKeyboard";
        public static readonly BindableProperty CanShowVirtualKeyboardProperty =
            BindableProperty.Create(CanShowVirtualKeyboardPropertyName, typeof(bool), typeof(ScanBarcodeEntry), true);
       
        public bool CanShowVirtualKeyboard
        {
            get { return (bool)GetValue(CanShowVirtualKeyboardProperty); }
            set {SetValue(CanShowVirtualKeyboardProperty, value); }
        }

        public const string OnEnterPressedCommandParameterPropertyName = "OnEnterPressedCommandParameter";
        public static readonly BindableProperty OnEnterPressedCommandParameterProperty =
            BindableProperty.Create(OnEnterPressedCommandParameterPropertyName, typeof(string), typeof(ScanBarcodeEntry), "");

        public string OnEnterPressedCommandParameter
        {
            get { return (string)GetValue(OnEnterPressedCommandParameterProperty); }
            set { SetValue(OnEnterPressedCommandParameterProperty, value);}
        }

        public event EventHandler<GenericEventArgs<FocusArgs>> OnFocused_TriggerRenderer;

		public new bool Focus()
        {
            var args = new GenericEventArgs<FocusArgs>(new FocusArgs { CouldFocusBeSet = false });
            OnFocused_TriggerRenderer?.Invoke(this, args);
            return args.EventData.CouldFocusBeSet;
        }
    }
}


