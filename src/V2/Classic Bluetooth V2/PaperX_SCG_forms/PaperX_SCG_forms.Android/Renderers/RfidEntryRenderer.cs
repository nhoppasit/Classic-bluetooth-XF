using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using System;
using System.ComponentModel;
using PaperX_SCG_forms.Droid.Renderers;
using PaperX_SCG_forms.Views.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PaperX_SCG_forms.Views.CustomControls.ScanBarcodeEntry), typeof(RfidEntryRenderer))]
namespace PaperX_SCG_forms.Droid.Renderers
{
    public class RfidEntryRenderer : EntryRenderer
    {
        private Android.Text.InputTypes _inputType;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control.Background = Resources.GetDrawable(Resource.Drawable.codeEntry);
            Control.SetTextColor(Android.Graphics.Color.Black);
            

            if (Control != null && e.OldElement == null)
            {
                ((Views.CustomControls.ScanBarcodeEntry) Element).OnHideKeyboard_TriggerRenderer += MyEntryRenderer_HideKeyboard;
                ((Views.CustomControls.ScanBarcodeEntry) Element).OnFocused_TriggerRenderer += MyEntryRenderer_FocusControl;

                _inputType = Control.InputType;

                Element.PropertyChanged += (sender, eve) =>
                {
                    if (eve.PropertyName == Views.CustomControls.ScanBarcodeEntry.CanShowVirtualKeyboardPropertyName)
                    {
                        if (!((Views.CustomControls.ScanBarcodeEntry) Element).CanShowVirtualKeyboard)
                            Control.InputType = 0;
                        else
                            Control.InputType = _inputType;
                    }
                };

                if (!(Element as Views.CustomControls.ScanBarcodeEntry).CanShowVirtualKeyboard)
                    Control.InputType = 0;

                Control.EditorAction += (sender, args) =>
                {
                    if (args.ActionId == ImeAction.ImeNull && args.Event.Action == KeyEventActions.Down)
                    {
                        (Element as Views.CustomControls.ScanBarcodeEntry)?.EnterPressed();
                    }
                };
            }
        }

        void MyEntryRenderer_HideKeyboard()
        {
            HideKeyboard();
        }

        void MyEntryRenderer_FocusControl(object sender, GenericEventArgs<FocusArgs> args)
        {
            args.EventData.CouldFocusBeSet = Control.RequestFocus();
            if (!((Views.CustomControls.ScanBarcodeEntry) Element).CanShowVirtualKeyboard)
                HideKeyboard();
        }

        public void HideKeyboard()
        {
            Control.RequestFocus();
            if (!((Views.CustomControls.ScanBarcodeEntry) Element).CanShowVirtualKeyboard)
            {
                Control.InputType = 0;
                InputMethodManager inputMethodManager = Control.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                inputMethodManager.HideSoftInputFromWindow(Control.WindowToken, HideSoftInputFlags.None);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var control = sender as Views.CustomControls.ScanBarcodeEntry;
            if (control == null) return;

            try
            {
                if (e.PropertyName == "IsFocused")
                {
                    Control.Background = control.IsFocused
                        ? Resources.GetDrawable(Resource.Drawable.focusBorderEntry)
                        : Resources.GetDrawable(Resource.Drawable.codeEntry);
                }
                if (e.PropertyName == "Text")
                {
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}