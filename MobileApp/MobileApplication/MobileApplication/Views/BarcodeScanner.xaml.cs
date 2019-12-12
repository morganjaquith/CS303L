using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace MobileApplication.Views
{
    public partial class BarcodeScanner : ContentPage
    {
        ZXingScannerPage scanPage;
        public BarcodeScanner()
        {
            InitializeComponent();
            ButtonScanDefault.Clicked += ButtonScanDefault_Clicked;
        }
        private async void ButtonScanDefault_Clicked(object sender, EventArgs e)
        {
            scanPage = new ZXingScannerPage();
            scanPage.OnScanResult += (result) => {
                scanPage.IsScanning = false;

                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopModalAsync();
                    DisplayAlert("Scanned Barcode", result.Text, "OK");
                });
            };
            await Navigation.PushModalAsync(scanPage);
        }
    }
}
