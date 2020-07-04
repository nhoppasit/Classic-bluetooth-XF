using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using PaperX_SCG_forms.Interface;

namespace PaperX_SCG_forms.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private IPageDialogService _pageDialogService;
        private IHandleSound _handleSound;
        private ICommand _appearCommand;
        private ICommand _scanBarcodeCommand;
        private string _barcode;

        public Action FocusBarCodeEntry { get; set; }
        public MainPageViewModel(IPageDialogService pageDialogService,IHandleSound handleSound)
        {
            _pageDialogService = pageDialogService;
            _handleSound = handleSound;
        }

        public string Barcode
        {
            get { return _barcode; }
            set { SetProperty(ref _barcode, value); }
        }

        public ICommand AppearCommand => _appearCommand ?? (_appearCommand = new DelegateCommand(Appear));

        private void Appear()
        {
            FocusBarCodeEntry();
        }

        public ICommand ScanBarcodeCommand =>
            _scanBarcodeCommand ?? (_scanBarcodeCommand = new DelegateCommand(ScanBarcode));

        public void ScanBarcode()
        {
            // do something when you scanbarcode



            //_pageDialogService.DisplayActionSheetAsync($"Barcode is [{Barcode}]",  "OK", "OK");
            Barcode = string.Empty;
            _handleSound.PlaySound();
            FocusBarCodeEntry();
            
        }
    }
}
