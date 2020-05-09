using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Newtonsoft.Json;

namespace ClassicBluetooth
{
    public class BaseSimpleReturn
    {
        public bool Flag { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }

        public BaseSimpleReturn(string code, string message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            if (code.Equals("00")) Flag = true;
            else Flag = false;
        }
    }

    public class CB_V1
    {
        private Java.Lang.String dataToSend;
        public Java.Lang.String DataToSend
        {
            get { return dataToSend; }
            set { dataToSend = value; }
        }

        private BluetoothAdapter mBluetoothAdapter = null;
        public BluetoothAdapter BluetoothAdapter
        {
            get { return mBluetoothAdapter; }
            private set { mBluetoothAdapter = value; }
        }

        private BluetoothSocket btSocket = null;
        public BluetoothSocket BluetoothSocket
        {
            get { return btSocket; }
            private set { btSocket = value; }
        }

        //Streams de lectura I/O
        private Stream outStream = null;    
        private Stream inStream = null;

        //MAC Address del dispositivo Bluetooth
        private static string address = "98:D3:C1:FD:41:51"; //98d3:c1:fd4151

        //Id Unico de comunicacion
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");



        /// <summary>
        /// ตรวจสอบอุปกรณ์ Bluetooth ของมือถือ
        /// </summary>
        /// <returns>BaseSimpleReturn</returns>
        private BaseSimpleReturn Check()
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            /* ---------------------------------------------------------------------------------------
             * VALIDATION
             * ---------------------------------------------------------------------------------------*/
            if (!mBluetoothAdapter.Enable())
            {
                return new BaseSimpleReturn("01", "Bluetooth ปิดการใช้งาน");
            }
            if (mBluetoothAdapter == null)
            {
                return new BaseSimpleReturn("01", "ไม่พบตัวแปลง Bluetooth");
            }

            /* ---------------------------------------------------------------------------------------
             * GOOD PASS
             * ---------------------------------------------------------------------------------------*/
            return new BaseSimpleReturn("00", "Bluetooth เปิดใช้งานแล้ว");
        }
    }
}