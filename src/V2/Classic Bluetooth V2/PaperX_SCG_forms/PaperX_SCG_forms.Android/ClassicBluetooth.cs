using Android.Content;
using System;
using Android.Bluetooth;
using Android.Util;
using Java.Util;
using Newtonsoft;
using Xamarin.Forms;
using Newtonsoft.Json;
using PaperX_SCG_forms;
using Java.IO;
using System.Diagnostics;
using Android.App;

[assembly: Dependency(typeof(ClassicBluetooth))]
public class ClassicBluetooth : IClassicBluetooth
{
    private BluetoothAdapter mBluetoothAdapter = null;
    private BluetoothSocket btSocket = null;

    public string BlinkOff()
    {
        System.IO.Stream outStream;
        try
        {
            outStream = btSocket.OutputStream;
        }
        catch (System.Exception ex1)
        {
            Log.Debug("PPX", ex1.Message);
            BasicReturn rEx1 = new BasicReturn() { Flag = false, Code = "OE1", Message = "Socket สร้างไม่สำเร็จ" };
            return JsonConvert.SerializeObject(rEx1, Formatting.Indented);
        }
        System.IO.Stream inStream;
        try
        {
            inStream = btSocket.InputStream;
        }
        catch (System.Exception ex2)
        {
            Log.Debug("PPX", ex2.Message);
            BasicReturn rEx2 = new BasicReturn() { Flag = false, Code = "OE1", Message = "Socket สร้างไม่สำเร็จ" };
            return JsonConvert.SerializeObject(rEx2, Formatting.Indented);
        }

        byte[] msgBuffer = new byte[] { 0xfd, 0x62, 0xca };
        try
        {
            outStream.Write(msgBuffer, 0, msgBuffer.Length);
        }
        catch (System.Exception ex2)
        {
            BasicReturn rEx1 = new BasicReturn() { Flag = false, Code = "OE2", Message = "ไม่สามารถส่งได้สำเร็จ" };
            return JsonConvert.SerializeObject(rEx1, Formatting.Indented);
        }

        BasicReturn r1 = new BasicReturn() { Flag = true, Code = "00", Message = "Blink off" };
        return JsonConvert.SerializeObject(r1, Formatting.Indented);
    }

    public string BlinkOn(int time)
    {
        System.IO.Stream outStream;
        try
        {
            outStream = btSocket.OutputStream;
        }
        catch (System.Exception ex1)
        {
            Log.Debug("PPX", ex1.Message);
            BasicReturn rEx1 = new BasicReturn() { Flag = false, Code = "OE1", Message = "Socket สร้างไม่สำเร็จ" };
            return JsonConvert.SerializeObject(rEx1, Formatting.Indented);
        }
        //System.IO.Stream inStream;
        //try
        //{
        //    inStream = btSocket.InputStream;
        //}
        //catch (System.Exception ex2)
        //{
        //    Log.Debug("PPX", ex2.Message);
        //    BasicReturn rEx2 = new BasicReturn() { Flag = false, Code = "OE1", Message = ex2.Message };
        //    return JsonConvert.SerializeObject(rEx2, Formatting.Indented);
        //}

        byte[] msgBuffer2 = System.Text.Encoding.ASCII.GetBytes(time.ToString());

        try
        {
            outStream.WriteByte(0xfd);
            outStream.WriteByte((byte)'B');
            outStream.Write(msgBuffer2, 0, msgBuffer2.Length);
            outStream.WriteByte(0xca);
        }
        catch (System.Exception ex2)
        {
            BasicReturn rEx1 = new BasicReturn() { Flag = false, Code = "OE2", Message = ex2.Message };
            return JsonConvert.SerializeObject(rEx1, Formatting.Indented);
        }

        //Stopwatch sw1 = new Stopwatch();
        //bool isTimeout = false;
        //int waitTtime = 1000;
        //sw1.Start();
        //string s = string.Empty;
        //while (sw1.ElapsedMilliseconds <= waitTtime)
        //{
        //    System.Threading.Thread.Sleep(10);
        //    byte[] buffer = new byte[1024];
        //    int bytes;
        //    bytes = inStream.Read(buffer, 0, buffer.Length);

        //    if (4 < bytes)
        //    {
        //        s = System.Text.Encoding.ASCII.GetString(buffer);
        //        break;
        //    }

        //    if (waitTtime <= sw1.ElapsedMilliseconds)
        //    {
        //        isTimeout = true;
        //        break;
        //    }
        //}
        //sw1.Stop();

        //if (isTimeout)
        //{
        //    BasicReturn rTO = new BasicReturn() { Flag = false, Code = "TO1", Message = "Read timeout!" };
        //    return JsonConvert.SerializeObject(rTO, Formatting.Indented);
        //}

        BasicReturn r1 = new BasicReturn() { Flag = true, Code = "00", Message = $"Blink ON of {time} sec" };
        return JsonConvert.SerializeObject(r1, Formatting.Indented);
    }

    public string Check()
    {
        try
        {
            if (mBluetoothAdapter == null)
                mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
        }
        catch { }

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

        //..
        foreach (var pairedDevice in BluetoothAdapter.DefaultAdapter.BondedDevices)
        {
            Log.Debug("PPX", $"Found device with name: {pairedDevice.Name} and MAC address: {pairedDevice.Address}");
            Trace.WriteLine($"Found device with name: {pairedDevice.Name} and MAC address: {pairedDevice.Address}");
        }

        // Good
        var r3 = new BasicReturn() { Flag = true, Code = "00", Message = "Bluetooth เปิดให้ใช้งานแล้ว" };
        return JsonConvert.SerializeObject(r3, Formatting.Indented);

    }
    public string Connect(string address, string uuid = "00001101-0000-1000-8000-00805F9B34FB")
    {
        BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
        Log.Debug("PPX", "กำลังเชื่อมต่อ " + device);

        mBluetoothAdapter.CancelDiscovery();
        try
        {
            UUID MY_UUID = UUID.FromString(uuid);
            btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
            btSocket.Connect();
            Log.Debug("PPX", "Socket เชื่อมต่อแล้ว");

            BasicReturn r1 = new BasicReturn() { Flag = true, Code = "00", Message = "Socket เชื่อมต่อแล้ว" };
            return JsonConvert.SerializeObject(r1, Formatting.Indented);
        }
        catch (System.Exception e)
        {
            Log.Debug("PPX", e.Message);
            try
            {
                btSocket.Close();
            }
            catch (System.Exception)
            {
                Log.Debug("PPX", "เชื่อมต่อ Socket ไม่ได้");
            }
            Log.Debug("PPX", "Socket สร้างไม่สำเร็จ");

            BasicReturn r2 = new BasicReturn() { Flag = false, Code = "SOC", Message = "Socket สร้างไม่สำเร็จ" };
            return JsonConvert.SerializeObject(r2, Formatting.Indented);
        }
    }

    public string Disconnect()
    {
        if (btSocket.IsConnected)
        {
            try
            {
                btSocket.Close();
            }
            catch (System.Exception ex)
            {
                var rEx = new BasicReturn() { Flag = false, Code = "SOC", Message = "ปิด Socket ไม่สำเร็จ" };
                return JsonConvert.SerializeObject(rEx, Formatting.Indented);
            }
        }

        var r3 = new BasicReturn() { Flag = true, Code = "00", Message = "Socket ปิดแล้ว" };
        return JsonConvert.SerializeObject(r3, Formatting.Indented);
    }

    public string ReadInfo()
    {
        System.IO.Stream outStream;
        try
        {
            outStream = btSocket.OutputStream;
        }
        catch (System.Exception ex1)
        {
            Log.Debug("PPX", ex1.Message);
            BasicReturn rEx1 = new BasicReturn() { Flag = false, Code = "OE1", Message = "Socket สร้างไม่สำเร็จ" };
            return JsonConvert.SerializeObject(rEx1, Formatting.Indented);
        }
        System.IO.Stream inStream;
        try
        {
            inStream = btSocket.InputStream;
        }
        catch (System.Exception ex2)
        {
            Log.Debug("PPX", ex2.Message);
            BasicReturn rEx2 = new BasicReturn() { Flag = false, Code = "OE1", Message = "Socket สร้างไม่สำเร็จ" };
            return JsonConvert.SerializeObject(rEx2, Formatting.Indented);
        }

        byte[] msgBuffer = new byte[] { 0xfe, 0x3f, 0xca };

        try
        {
            outStream.Write(msgBuffer, 0, msgBuffer.Length);
        }
        catch (System.Exception ex2)
        {
            BasicReturn rEx1 = new BasicReturn() { Flag = false, Code = "OE2", Message = "ไม่สามารถส่งได้สำเร็จ" };
            return JsonConvert.SerializeObject(rEx1, Formatting.Indented);
        }

        Stopwatch sw1 = new Stopwatch();
        bool isTimeout = false;
        int waitTtime = 1000;
        sw1.Start();
        string s = string.Empty;
        while (sw1.ElapsedMilliseconds <= waitTtime)
        {
            System.Threading.Thread.Sleep(150);
            byte[] buffer = new byte[1024];
            int bytes;
            bytes = inStream.Read(buffer, 0, buffer.Length);

            if (4 < bytes)
            {
                s = System.Text.Encoding.ASCII.GetString(buffer);
                break;
            }

            if (waitTtime <= sw1.ElapsedMilliseconds)
            {
                isTimeout = true;
                break;
            }
        }
        sw1.Stop();

        if (isTimeout)
        {
            BasicReturn rTO = new BasicReturn() { Flag = false, Code = "TO1", Message = "Read timeout!" };
            return JsonConvert.SerializeObject(rTO, Formatting.Indented);
        }

        BasicReturn r1 = new BasicReturn() { Flag = true, Code = "00", Message = s };
        return JsonConvert.SerializeObject(r1, Formatting.Indented);
    }
}
