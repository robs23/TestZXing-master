using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace TestZXing.Classes
{
    public class QrHandler
    {
        public bool IsSuccessful { get; set; } = true;
        public string Result { get; set; } = null;

        public ZXingScannerPage scanPage { get; set; }
        public QrHandler()
        {

        }

        public async Task<string> Scan()
        {
            try
            {
                PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();

                if (status != PermissionStatus.Granted)
                {
                    status = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                }
                if (status != PermissionStatus.Granted)
                {
                    IsSuccessful = false;
                    Result = "Skanowanie kodu QR wymaga uprawnień do użycia kamery. Użytkownik odmówił tego uprawnienia. Spróbuj jeszcze raz i przyznaj odpowiednie uprawnienie";
                }
                else
                {
                    var options = new ZXing.Mobile.MobileBarcodeScanningOptions
                    {
                        PossibleFormats = new List<ZXing.BarcodeFormat>
                    {
                        ZXing.BarcodeFormat.QR_CODE
                    },
                        TryHarder = false,
                        AutoRotate = false,
                        TryInverted = false,
                    };
                    scanPage = new ZXingScannerPage();
                    scanPage.AutoFocus();
                    using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1))
                    {
                        scanPage.OnScanResult += (result) =>
                        {
                            //DateTime _start = DateTime.Now;
                            scanPage.IsScanning = false;

                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                Application.Current.MainPage.Navigation.PopAsync();
                                try
                                {

                                    Result = result.Text;

                                }
                                catch (Exception ex)
                                {
                                    Result = ex.Message;
                                }
                                semaphoreSlim.Release();
                            });
                        };
                        await Application.Current.MainPage.Navigation.PushAsync(scanPage);
                        await semaphoreSlim.WaitAsync();
                    }                
                    
                }

            }
            catch (Exception ex)
            {

            }

            return Result;
        }
    }
}
