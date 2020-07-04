using Android.Content;
using System;
using Android.Bluetooth;
using Android.Util;
using Java.Util;
using Newtonsoft;
using Xamarin.Forms;
using CB_V1;
using Newtonsoft.Json;

[assembly: Dependency(typeof(ClassicBluetooth))]
namespace CB_V1
{
    public class ClassicBluetooth : IClassicBluetooth
    {
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;

        public string Check()
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (!mBluetoothAdapter.Enable())
            {
                var r1 = new BasicReturn() { Flag = false, Code = "DIS", Message = "Bluetooth ไม่ได้เปิด!" };
                return JsonConvert.SerializeObject(r1, Formatting.Indented);
            }

            if (mBluetoothAdapter == null)
            {
                var r2 = new BasicReturn() { Flag = false, Code = "NUL", Message = "ไม่พบอุปกรณ์ Bluetooth!" };
                return JsonConvert.SerializeObject(r2, Formatting.Indented);
            }

            // Good
            var r3 = new BasicReturn() { Flag = true, Code = "00", Message = "Bluetooth เปิดให้ใช้งานแล้ว" };
            return JsonConvert.SerializeObject(r3, Formatting.Indented);

        }
    }
}