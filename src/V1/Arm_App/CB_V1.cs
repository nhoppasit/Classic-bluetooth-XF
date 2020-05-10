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
        private string mAddress = "98:D3:C1:FD:41:51"; //98d3:c1:fd4151
        public string Address
        {
            get { return mAddress; }
            set { mAddress = value; }
        }

        //Id Unico de comunicacion
        private  UUID mUuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
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
        private BaseSimpleReturn Check()
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
                return new BaseSimpleReturn("01", "ไม่พบตัวแปลง Bluetooth");
            }

            /* ---------------------------------------------------------------------------------------
             * GOOD PASS
             * ---------------------------------------------------------------------------------------*/
            return new BaseSimpleReturn("00", "Bluetooth เปิดใช้งานแล้ว");
        }

        public void Connect()
        {
            //Iniciamos la conexion con el arduino
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(mAddress);

            System.Console.WriteLine("กำลังเชื่อมต่อ " + device);

            //Indicamos al adaptador que ya no sea visible
            mBluetoothAdapter.CancelDiscovery();
            try
            {
                //Inicamos el socket de comunicacion con el arduino
                btSocket = device.CreateRfcommSocketToServiceRecord(mUuid);

                //Conectamos el socket
                btSocket.Connect();
                System.Console.WriteLine("Conexion Correcta");
            }
            catch (System.Exception e)
            {
                //en caso de generarnos error cerramos el socket
                Console.WriteLine(e.Message);
                try
                {
                    btSocket.Close();
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Imposible Conectar");
                }
                System.Console.WriteLine("Socket Creado");
            }
            //Una vez conectados al bluetooth mandamos llamar el metodo que generara el hilo
            //que recibira los datos del arduino
            beginListenForData();
            //NOTA envio la letra e ya que el sketch esta configurado para funcionar cuando
            //recibe esta letra.
            //  dataToSend = new Java.Lang.String("e");
            //writeData(dataToSend);
        }


    }
}