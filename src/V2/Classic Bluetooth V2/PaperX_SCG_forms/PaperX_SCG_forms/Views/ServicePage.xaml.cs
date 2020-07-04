using Android.Util;
using Java.Lang;
using Newtonsoft.Json;
using PaperX_SCG_forms.Classes;
using PaperX_SCG_forms.ViewModels;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaperX_SCG_forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServicePage : ContentPage
    {
        const int Timeout_to_main_page = 20/*sec*/;
        public ServicePage()
        {
            InitializeComponent();

            App.Hardware = new HardwareClass();
            App.SaveLog(DateTime.Now, "PPX", "start application");

            lblWebServiceStatus.Text = "Initializing...";
            lblHardwaresConnectionStatus.Text = "Initializing...";

            Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    layoutCheckZone.IsVisible = false;
                });
                //App.SaveLog(DateTime.Now, "PPX", "CheckWsProcAsync");
                //CheckWsProcAsync();
                //App.SaveLog(DateTime.Now, "PPX", "CheckHwProcAsync");
                //CheckHwProcAsync();
                Device.BeginInvokeOnMainThread(() =>
                {
                    layoutCheckZone.IsVisible = true;
                });
            });

            bluetooth = DependencyService.Get<IClassicBluetooth>();
        }

        protected override async void OnAppearing()
        {

        }

        volatile bool StopFlag = false;

        #region Page events
        private void Page_Tapped(object sender, EventArgs e)
        {
            StopFlag = true;
        }

        #endregion

        #region Pages link
        private void BtnMainPage_Clicked(object sender, EventArgs e)
        {
            StopFlag = true;
            //App.Current.MainPage = new MainPage();
        }

        #endregion

        #region Checker

        private void BtnCheckWs_Clicked(object sender, EventArgs e)
        {
            StopFlag = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                layoutCheckZone.IsVisible = false;
            });
            //CheckWsProcAsync();
            Device.BeginInvokeOnMainThread(() =>
            {
                layoutCheckZone.IsVisible = true;
            });
        }

        private void BtnCheckHw_Clicked(object sender, EventArgs e)
        {
            StopFlag = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                layoutCheckZone.IsVisible = false;
            });
            //CheckHwProcAsync();
            BtConnect();
            Device.BeginInvokeOnMainThread(() =>
            {
                layoutCheckZone.IsVisible = true;
            });
        }

        #endregion

        public IClassicBluetooth bluetooth;

        void BtConnect()
        {
            var r1 = bluetooth.Check();
            BasicReturn r1Obj = JsonConvert.DeserializeObject<BasicReturn>(r1);
            Log.Debug("PPX", r1Obj.Message);

            string address = "98:D3:C1:FD:41:51";
            string uuid = "00001101-0000-1000-8000-00805F9B34FB";
            var r2 = bluetooth.Connect(address, uuid);
            BasicReturn r2Obj = JsonConvert.DeserializeObject<BasicReturn>(r2);
            Log.Debug("PPX", r2Obj.Message);

            var rInfo = bluetooth.ReadInfo();
            BasicReturn rInfoObj = JsonConvert.DeserializeObject<BasicReturn>(rInfo);
            Log.Debug("PPX", rInfoObj.Message);

            //var rDis = bluetooth.Disconnect();
            //BasicReturn rDisObj = JsonConvert.DeserializeObject<BasicReturn>(rDis);
            //Log.Debug("PPX", rDisObj.Message);

        }

        #region HW control panel

        bool MachineFlag = false;


        private async void BtnReadWeight_Clicked(object sender, EventArgs e)
        {
            StopFlag = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                layoutAlarm.IsVisible = false;
            });
            //await WeightProc();
        }

        private async void BtnHomeShutter_Clicked(object sender, EventArgs e)
        {
            StopFlag = true;

            //var r1 = bluetooth.Check();
            //BasicReturn r1Obj = JsonConvert.DeserializeObject<BasicReturn>(r1);
            //Log.Debug("PPX", r1Obj.Message);

            //string address = "98:D3:C1:FD:41:51";
            //string uuid = "00001101-0000-1000-8000-00805F9B34FB";
            //var r2 = bluetooth.Connect(address, uuid);
            //BasicReturn r2Obj = JsonConvert.DeserializeObject<BasicReturn>(r2);
            //Log.Debug("PPX", r2Obj.Message);

            var rInfo = bluetooth.BlinkOn(100);
            BasicReturn rInfoObj = JsonConvert.DeserializeObject<BasicReturn>(rInfo);
            Log.Debug("PPX", rInfoObj.Message);

            //var rDis = bluetooth.Disconnect();
            //BasicReturn rDisObj = JsonConvert.DeserializeObject<BasicReturn>(rDis);
            //Log.Debug("PPX", rDisObj.Message);
        }

        private async void BtnOpenShutter_Clicked(object sender, EventArgs e)
        {
            StopFlag = true;

            //var r1 = bluetooth.Check();
            //BasicReturn r1Obj = JsonConvert.DeserializeObject<BasicReturn>(r1);
            //Log.Debug("PPX", r1Obj.Message);

            //string address = "98:D3:C1:FD:41:51";
            //string uuid = "00001101-0000-1000-8000-00805F9B34FB";
            //var r2 = bluetooth.Connect(address, uuid);
            //BasicReturn r2Obj = JsonConvert.DeserializeObject<BasicReturn>(r2);
            //Log.Debug("PPX", r2Obj.Message);

            var rInfo = bluetooth.BlinkOn(1000);
            BasicReturn rInfoObj = JsonConvert.DeserializeObject<BasicReturn>(rInfo);
            Log.Debug("PPX", rInfoObj.Message);

            //var rDis = bluetooth.Disconnect();
            //BasicReturn rDisObj = JsonConvert.DeserializeObject<BasicReturn>(rDis);
            //Log.Debug("PPX", rDisObj.Message);
        }

        private async void BtnCloseShutter_Clicked(object sender, EventArgs e)
        {
            StopFlag = true;
            
            //var r1 = bluetooth.Check();
            //BasicReturn r1Obj = JsonConvert.DeserializeObject<BasicReturn>(r1);
            //Log.Debug("PPX", r1Obj.Message);

            //string address = "98:D3:C1:FD:41:51";
            //string uuid = "00001101-0000-1000-8000-00805F9B34FB";
            //var r2 = bluetooth.Connect(address, uuid);
            //BasicReturn r2Obj = JsonConvert.DeserializeObject<BasicReturn>(r2);
            //Log.Debug("PPX", r2Obj.Message);

            var rInfo = bluetooth.BlinkOff();   
            BasicReturn rInfoObj = JsonConvert.DeserializeObject<BasicReturn>(rInfo);
            Log.Debug("PPX", rInfoObj.Message);

            //var rDis = bluetooth.Disconnect();
            //BasicReturn rDisObj = JsonConvert.DeserializeObject<BasicReturn>(rDis);
            //Log.Debug("PPX", rDisObj.Message);
        }

        private async void BtnReadSensors_Clicked(object sender, EventArgs e)
        {
            StopFlag = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                layoutAlarm.IsVisible = false;
            });
            //await ReadAllSensorProc();
        }

        public async Task WeightProc()
        {
            try
            {
                if (MachineFlag)
                {
                    Log.Debug("PPX", "Mac in used! WeightProc()");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        layoutAlarm.IsVisible = true;
                        lblAlarm.TextColor = Color.Red;
                        lblAlarm.Text = "Machine Flag แจ้งว่าเครื่องทำงานอยู่!";
                    });
                    return;
                }
                MachineFlag = true;
                if (App.Hardware.iUsb != null)
                {
                    //-------------------------------------------------
                    // Check hardware.
                    //-------------------------------------------------
                    if (App.Hardware.HardwareData.IndicatorDeviceIndex < 0)
                    {
                        Log.Debug("PPX", "Scale indicator not found!");
                        MachineFlag = false;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            layoutAlarm.IsVisible = true;
                            lblAlarm.TextColor = Color.Red;
                            lblAlarm.Text = "Scale indicator not found!";
                        });
                        globalVariables.ErrorCode = "41";
                        App.SaveLog(DateTime.Now, "PPX", globalVariables.ticket + " = 40:MCU interfacing fault! พบความผิดพลาดในการเชื่อมต่อกับ MCU.");
                        var resultWs40 = globalVariables.ws.ws_SendMachineStatus(globalVariables.ticket, 2, "40:MCU interfacing fault! พบความผิดพลาดในการเชื่อมต่อกับ MCU.", Convert.ToInt32(globalVariables.ProductID), Convert.ToDouble(globalVariables.percentBinFull), Convert.ToDouble(globalVariables.weightBinFull));
                        var SendMachineStatus = JsonConvert.DeserializeObject<t_SendMachineStatus>(resultWs40);
                        return;
                    }

                    //---------------------------------------------------
                    // Shutter homing
                    //---------------------------------------------------
                    //..??????
                    //..Tick-Tok Start here! If soft timeout, StopFlag = true;
                    string ret = await App.Hardware.iUsb.Weighting(App.Hardware.HardwareData.IndicatorDeviceIndex, 20000).ConfigureAwait(false);
                    //
                    // Excecute
                    if (ret.Contains("00:"))//OK
                    {
                        Log.Debug("PPX", App.Hardware.iUsb.LastResultMessage);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            layoutAlarm.IsVisible = true;
                            lblAlarm.TextColor = Color.Blue;
                            lblAlarm.Text = $"Weight = {double.Parse(ret.Replace("00:", "")) } kg";
                        });
                        MachineFlag = false;
                        return;
                    }
                    else//Hardware not good response!
                    {
                        Log.Debug("PPX", $"Hardware response is not good! Result = {ret}.");
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            layoutAlarm.IsVisible = true;
                            lblAlarm.Text = ret;
                        });
                        MachineFlag = false;
                        globalVariables.ErrorCode = "51";
                        App.SaveLog(DateTime.Now, "PPX", globalVariables.ticket + " = 50:Shutter home position fault!  พบความผิดพลาดในการตงั้ต าแหน่ง ประตูไปยงัจดุเริ่มต้น (ปิด)");
                        var resultWs50 = globalVariables.ws.ws_SendMachineStatus(globalVariables.ticket, 2, "50:Shutter home position fault!  พบความผิดพลาดในการตงั้ต าแหน่ง ประตูไปยงัจดุเริ่มต้น (ปิด)", Convert.ToInt32(globalVariables.ProductID), Convert.ToDouble(globalVariables.percentBinFull), Convert.ToDouble(globalVariables.weightBinFull));
                        var SendMachineStatus = JsonConvert.DeserializeObject<t_SendMachineStatus>(resultWs50);
                        return;
                    }
                }
                else// USB fault!
                {
                    MachineFlag = false;
                    Log.Debug("PPX", "USB fault!");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        layoutAlarm.IsVisible = true;
                        lblAlarm.TextColor = Color.Red;
                        lblAlarm.Text = "USB-serial I/O fault!";
                    });
                    globalVariables.ErrorCode = "51";
                    App.SaveLog(DateTime.Now, "PPX", globalVariables.ticket + " = 50:USB fault!");
                    var resultWs40 = globalVariables.ws.ws_SendMachineStatus(globalVariables.ticket, 2, "50:USB fault!", Convert.ToInt32(globalVariables.ProductID), Convert.ToDouble(globalVariables.percentBinFull), Convert.ToDouble(globalVariables.weightBinFull));
                    var SendMachineStatus = JsonConvert.DeserializeObject<t_SendMachineStatus>(resultWs40);
                    return;
                }
            }
            catch (Java.Lang.Exception exc)
            {
                MachineFlag = false;
                Log.Debug("PPX", $"{exc.Message}\r\n{exc.StackTrace}");
                Device.BeginInvokeOnMainThread(() =>
                {
                    layoutAlarm.IsVisible = true;
                    lblAlarm.TextColor = Color.Red;
                    lblAlarm.Text = $"{exc.Message}\r\n{exc.StackTrace}";
                });
                globalVariables.ErrorCode = "51";
                App.SaveLog(DateTime.Now, "PPX", globalVariables.ticket + $" = 50:{exc.Message}\r\n{exc.StackTrace}");
                var resultWs40 = globalVariables.ws.ws_SendMachineStatus(globalVariables.ticket, 2, $"50:{exc.Message}\r\n{exc.StackTrace}", Convert.ToInt32(globalVariables.ProductID), Convert.ToDouble(globalVariables.percentBinFull), Convert.ToDouble(globalVariables.weightBinFull));
                var SendMachineStatus = JsonConvert.DeserializeObject<t_SendMachineStatus>(resultWs40);
            }

        }

        #endregion

        private void btnExitApp_Clicked(object sender, EventArgs e)
        {
            JavaSystem.Exit(0);
        }

    }
}