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
using Android.Util;
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

    /// <summary>
    /// Classic Bluetoot
    /// </summary>
    public class CB_V1
    {
        private Java.Lang.String dataToSend;
        public Java.Lang.String DataToSend
        {
            get { return dataToSend; }
            set { dataToSend = value; }
        }

        private BluetoothDevice device = null;

        private BluetoothAdapter mBluetoothAdapter = null;
        public BluetoothAdapter BluetoothAdapter
        {
            get { return mBluetoothAdapter; }
        }

        private BluetoothSocket btSocket = null;
        public BluetoothSocket BluetoothSocket
        {
            get { return btSocket; }
        }

        //private Stream outStream = null;
        //private Stream inStream = null;

        //MAC Address del dispositivo Bluetooth
        private string mAddress = null;// "98:D3:C1:FD:41:51"; //98d3:c1:fd4151
        public string Address
        {
            get { return mAddress; }
            set { mAddress = value; }
        }

        //Id Unico de comunicacion
        private UUID mUuid = null;// UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        public UUID UUID
        {
            get { return mUuid; }
            set { mUuid = value; }
        }

        public CB_V1(string address, string sUuid)
        {
            this.mAddress = address;
            this.mUuid = UUID.FromString(sUuid);
        }

        /// <summary>
        /// ตรวจสอบอุปกรณ์ Bluetooth ของมือถือ
        /// </summary>
        /// <returns>BaseSimpleReturn</returns>
        public BaseSimpleReturn Check()
        {
            Log.Debug("CB_V1", $"Check adapter");
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
                return new BaseSimpleReturn("02", "ไม่พบตัวแปลง Bluetooth");
            }

            /* ---------------------------------------------------------------------------------------
             * GOOD PASS
             * ---------------------------------------------------------------------------------------*/
            return new BaseSimpleReturn("00", "Bluetooth เปิดใช้งานแล้ว");
        }

        public BaseSimpleReturn Connect()
        {
            bool deviceCreated = false;
            bool socketCreated = false;
            //bool inboundStreamCreated = false;

            /* ......................................................
             * Create device
             * ......................................................*/
            try
            {
                Log.Debug("CB_V1", $"กำลังสร้าง BluetoothDevice");
                device = mBluetoothAdapter.GetRemoteDevice(mAddress);
                Log.Debug("CB_V1", $"{device} สร้างเสร็จแล้ว");
                deviceCreated = true;
            }
            catch (Exception ex)
            {
                Log.Debug("CB_V1", $"{ex.Message}\r\n{ex.StackTrace}");
            }
            if (!deviceCreated)
            {
                return new BaseSimpleReturn("01", "ซอฟแวร์ Bluetooth ไม่พร้อมใช้งาน!");
            }

            /* ......................................................
             * Create socket
             * ......................................................*/
            try
            {
                Log.Debug("CB_V1", $"กำลังสร้าง Socket และเชื่อมต่อ");
                mBluetoothAdapter.CancelDiscovery();
                btSocket = device.CreateRfcommSocketToServiceRecord(mUuid);
                Log.Debug("CB_V1", $"{btSocket} สร้างแล้ว");
                btSocket.Connect();
                Log.Debug("CB_V1", $"{Address} เชื่อมต่อแล้ว");
                socketCreated = true;
            }
            catch (System.Exception e)
            {
                Log.Debug("CB_V1", $"{e.Message}\r\n{e.StackTrace}");
            }
            if (!socketCreated)
            {
                try
                {
                    Log.Debug("CB_V1", $"กำลังปิด Socket");
                    btSocket.Close();
                    Log.Debug("CB_V1", $"ปิด Socket แล้ว");
                }
                catch (System.Exception ee)
                {
                    Log.Debug("CB_V1", $"{ee.Message}\r\n{ee.StackTrace}");
                    Log.Debug("CB_V1", $"ปิด Socket ไม่ได้");
                }
                return new BaseSimpleReturn("02", "ไม่สามารถเชื่อมต่อ Bluetooth ได้");
            }

            ///* ......................................................
            // * Create input stream
            // * ......................................................*/
            //try
            //{
            //    Log.Debug("CB_V1", $"กำลังสร้าง Input stream.");
            //    inStream = btSocket.InputStream;
            //    Log.Debug("CB_V1", $"Input stream สร้างแล้ว");
            //    inboundStreamCreated = true;
            //}
            //catch (System.IO.IOException ex)
            //{
            //    Log.Debug("CB_V1", $"{ex.Message}\r\n{ex.StackTrace}");
            //}
            //if (!inboundStreamCreated)
            //{
            //    Log.Debug("CB_V1", $"สร้าง Input stream ไม่ได้");
            //    return new BaseSimpleReturn("02", "ไม่สามารถเชื่อมต่อ Bluetooth ได้");
            //}

            /* ......................................................
             * Good pass
             * ......................................................*/
            return new BaseSimpleReturn("00", "เชื่อมต่อ Bluetooth แล้ว");


            //Una vez conectados al bluetooth mandamos llamar el metodo que generara el hilo
            //que recibira los datos del arduino
            //beginListenForData();
            //NOTA envio la letra e ya que el sketch esta configurado para funcionar cuando
            //recibe esta letra.
            //  dataToSend = new Java.Lang.String("e");
            //writeData(dataToSend);
        }

        public BaseSimpleReturn Disconnect()
        {
            if (btSocket.IsConnected)
            {
                try
                {
                    btSocket.Close();
                    Log.Debug("CB_V1", $"ปิด Socket แล้ว");

                    /* ---------------------------------------------------------------------------------------
                     * GOOD PASS
                     * ---------------------------------------------------------------------------------------*/
                    return new BaseSimpleReturn("00", "Bluetooth socket ปิดแล้ว");
                }
                catch (System.Exception ex)
                {
                    Log.Debug("CB_V1", $"{ex.Message}\r\n{ex.StackTrace}");
                    return new BaseSimpleReturn("01", "Socket อาจปิดไม่สำเร็จ!");
                }

            }
            else
            {
                return new BaseSimpleReturn("00", "Socket ปิดอยู่แล้ว!");
            }
        }
                
    }
}