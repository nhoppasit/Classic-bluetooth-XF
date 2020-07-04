using Android.Util;
using Newtonsoft.Json;
using PaperX_SCG_forms.Classes;
using PaperX_SCG_forms.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperX_SCG_forms.Classes
{
    public class HardwareClass
    {
        public HardwareDataClass HardwareData { get; set; }
        public IUsb iUsb;
        int _lastResult = 0; public int LastResult { get { return _lastResult; } private set { _lastResult = value; } }
        string _lastResultMessage = ""; public string LastResultMessage { get { return _lastResultMessage; } private set { _lastResultMessage = value; } }


        public HardwareClass()
        {
            JsonNvram jsonNvram = new JsonNvram("PaperX", "hardware");
            jsonNvram.CheckPath();

            HardwareData = JsonConvert.DeserializeObject<HardwareDataClass>(jsonNvram.Read());
            if (HardwareData == null)
            {
                HardwareData = new HardwareDataClass();
            }

            iUsb = DependencyService.Get<IUsb>();
        }

        bool MachineFlag = false;
        public async Task<int> FindHardwares()
        {
            try
            {
                LastResult = -99; LastResultMessage = "Start find hardware.";

                /* -------------------------------------------------------
                 * ตรวจสอบความพร้อม
                 -------------------------------------------------------*/
                if (MachineFlag)
                {
                    LastResult = -1; LastResultMessage = "มีการเรียกใช้ Hardware อยู่ ต้องยกเลิก!";
                    return -1;
                }

                /* -------------------------------------------------------
                 * ตรวจสอบการเชื่อมต่อ USB ของ class
                 -------------------------------------------------------*/
                MachineFlag = true;
                //var iUsb = DependencyService.Get<IUsb>();
                if (App.Hardware.iUsb == null)
                {
                    MachineFlag = false;
                    Log.Debug("PPX", "USB fault! iUSB = null!");

                    globalVariables.ErrorCode = "50";
                    var macStatusResult = globalVariables.ws.ws_SendMachineStatus(globalVariables.ticket, 2,
                        "50:USB fault!",
                        Convert.ToInt32(globalVariables.ProductID),
                        Convert.ToDouble(globalVariables.percentBinFull),
                        Convert.ToDouble(globalVariables.weightBinFull));
                    var SendMachineStatus = JsonConvert.DeserializeObject<t_SendMachineStatus>(macStatusResult);

                    LastResult = -2; LastResultMessage = "USB class ผิดพลาด iUSB = null!";
                    return -2;
                }

                /* -------------------------------------------------------
                 * USB พร้อมใช้งาน
                 * ต่อไปดำเนินการเรียกดูรายการ Hardware 
                 -------------------------------------------------------*/
                int[] devIndexes = await iUsb.FindDevices();
                LastResult = iUsb.LastResult; LastResultMessage = iUsb.LastResultMessage;
                if (LastResult == 0)
                {
                    HardwareData.McuDeviceIndex = devIndexes[0];
                    HardwareData.IndicatorDeviceIndex = devIndexes[1];
                    HardwareData.Ready = true;
                }
                else
                {
                    HardwareData.McuDeviceIndex = -1;
                    HardwareData.IndicatorDeviceIndex = -1;
                    HardwareData.Ready = false;
                }

                /*-------------------------------------------------
                 * แจ้งเตือนไปยัง Server
                 * Check MCU.
                -------------------------------------------------*/
                if (HardwareData.McuDeviceIndex < 0)
                {
                    Log.Debug("PPX", "MCU not found!");
                    MachineFlag = false;
                    globalVariables.ErrorCode = "40";
                    var result1 = globalVariables.ws.ws_SendMachineStatus(globalVariables.ticket, 2,
                        "40:MCU interfacing fault! พบความผิดพลาดในการเชื่อมต่อกับ MCU.",
                        Convert.ToInt32(globalVariables.ProductID),
                        Convert.ToDouble(globalVariables.percentBinFull),
                        Convert.ToDouble(globalVariables.weightBinFull));
                    var SendMachineStatus = JsonConvert.DeserializeObject<t_SendMachineStatus>(result1);

                }

                /*-------------------------------------------------
                 * แจ้งเตือนไปยัง Server
                 * Check indicator.
                -------------------------------------------------*/
                if (HardwareData.IndicatorDeviceIndex < 0)
                {
                    Log.Debug("PPX", "Indicator not found!");
                    MachineFlag = false;
                    //  App.Current.MainPage = new MainPage();

                    globalVariables.ErrorCode = "41";
                    var result2 = globalVariables.ws.ws_SendMachineStatus(globalVariables.ticket, 2,
                        "41:Scale indicator’s interfacing fault! พบความผิดพลาดในการเชื่อมต่อกีบเครื่องชั่ง ",
                        Convert.ToInt32(globalVariables.ProductID),
                        Convert.ToDouble(globalVariables.percentBinFull),
                        Convert.ToDouble(globalVariables.weightBinFull));
                    var SendMachineStatus = JsonConvert.DeserializeObject<t_SendMachineStatus>(result2);
                }

                MachineFlag = false;
                if (!HardwareData.Ready)
                {
                    //App.Current.MainPage = new NoService("งดให้บริการ");
                    LastResult = -3; LastResultMessage += "\r\nผิดพลาด! ฮาร์ดแวร์ไม่พร้อมใช้งาน";
                    return -3;
                }
                else
                {
                    LastResult = 0; LastResultMessage = "พบฮาร์ดแวร์ตามปกติ";
                    return 0;
                }
            }
            catch (Exception exc)
            {
                MachineFlag = false;
                Log.Debug("PPX", exc.Message);
                LastResult = -90;
                LastResultMessage = exc.Message;
                throw exc;
            }

        }


    }
}
