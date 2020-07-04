using Android.Content;
using Android.Hardware.Usb;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Extensions;
using PaperX_SCG_forms;
using PaperX_SCG_forms.Droid;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.Hardware.Usb;
using Android.Content;
using System.Collections.Generic;
using Android.Util;
using Hoho.Android.UsbSerial.Util;
using System;
using System.Text;
using System.Diagnostics;

[assembly: Dependency(typeof(Usb))]
public class Usb : IUsb
{
    public UsbSerialPort Port1 { get; set; }
    public UsbSerialPort Port2 { get; set; }
    SerialInputOutputManager SerialIoManager1;
    SerialInputOutputManager SerialIoManager2;
    public UsbManager usbManager { get; set; }

    const int READ_1_WAIT_MILLIS = 200;
    const int READ_2_WAIT_MILLIS = 400;
    const int DEFAULT_BUFFERSIZE = 4096;
    const int TIMEOUT_SEEK = 2000;
    const int TIMEOUT_READ_MCU = 2000;
    const int TIMEOUT_READ_INDICATOR = 2000;

    byte[] McuBuffer;
    byte[] IndicatorBuffer;
    byte[] Buffer1;
    byte[] Buffer2;

    public volatile bool StopFlag = false;

    int _lastResult = 0; public int LastResult { get { return _lastResult; } private set { _lastResult = value; } }
    string _lastResultMessage = ""; public string LastResultMessage { get { return _lastResultMessage; } private set { _lastResultMessage = value; } }

    public static IList<IUsbSerialDriver> drivers;

    public Usb()
    {
        drivers = null;
    }

    public async Task Stop()
    {
        StopFlag = true;
    }

    public async Task<int[]> FindDevices()
    {
        try
        {
            LastResult = -99; LastResultMessage = "Start find devices in USB class.";

            string text1 = "", text2 = "";

            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            /* ----------------------------------------------------------------
             * USB device validation!
             * ไม่พบ device เลย
             ----------------------------------------------------------------*/
            if (drivers.Count <= 0)
            {
                Log.Debug("PPX", "USB devices not found!");
                LastResult = -1; LastResultMessage = "ไม่พบ USB device เลย!";
                return new int[] { -1, -1 };
            }

            /* ----------------------------------------------------------------
             * USB device validation!
             * พบ device ไม่ครบจำนวน 2 
             *  - แจ้ง log กลับไปว่าเจออะไร แล้วจบ
             ----------------------------------------------------------------*/
            if (drivers.Count < 2)
            {
                Port1 = drivers[0].Ports[0];
                text1 = $"พบ USB-Serial device หนึ่งตัว ยังไม่ครบสองตัว\r\n" +
                   $"MFN: {Port1.Driver.Device.ManufacturerName}, \r\n" +
                   $"VID: {Port1.Driver.Device.VendorId}, \r\n" +
                   $"PID: {Port1.Driver.Device.ProductId}, \r\n" +
                   $"DID: {Port1.Driver.Device.DeviceId}, \r\n" +
                   $"Name: {Port1.Driver.Device.DeviceName}";
                Log.Debug("PPX", text1);
                LastResult = -2; LastResultMessage = "พบ device ไม่ครบจำนวน 2\r\nขอแจ้งกลับว่าเจอ dev อะไร แล้วจบคำสั่ง";
                return new int[] { 0, -1 };
            }

            /* ----------------------------------------------------------------
             * เริ่มการค้นหา USB ที่ต้องการ (คำสั่งทำรวดเดียวรอบเดียว)
             *      1. MCU
             *      2. Scale indicator
             *      3. Don't care for others
             ----------------------------------------------------------------*/
            int iDev = 0;
            Port1 = drivers[iDev].Ports[0]; // dev-> 1: try MCU first.
            text1 = $"USB device found on port 1. \r\n" +
                   $"MFN: {Port1.Driver.Device.ManufacturerName}, \r\n" +
                   $"VID: {Port1.Driver.Device.VendorId}, \r\n" +
                   $"PID: {Port1.Driver.Device.ProductId}, \r\n" +
                   $"DID: {Port1.Driver.Device.DeviceId}, \r\n" +
                   $"Name: {Port1.Driver.Device.DeviceName}";
            Log.Debug("PPX", text1);
            SerialIoManager1 = new SerialInputOutputManager(Port1)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };
            Log.Info("PPX", "Starting MCU(Try) IO manager...");
            var permissionGranted1 = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
            if (!permissionGranted1)
            {
                if (SerialIoManager1 != null) SerialIoManager1.Close();
                Log.Info("PPX", "MCU(try) Permission denined!");
                LastResult = -3; LastResultMessage = "MCU(try) USB permission denined!";
                return new int[] { -1, -1 };
            }

            bool McuFoundFlag = false;
            bool IndicatorFoundFlag = false;
            int McuDeviceIndex = -1;
            int IndicatorDeviceIndex = -1;

            /* ------------------------------------------------------
             * เปิดพอร์ต เริ่มจาก MCU
             ------------------------------------------------------*/
            Buffer1 = new byte[DEFAULT_BUFFERSIZE];
            SerialIoManager1.Open(usbManager); // -> OPEN
            System.Threading.Thread.Sleep(100); // -> WAIT FOR HARDWARE RESET VIA UARTS

            /* ------------------------------------------------------
             * ส่งข้อความถึง MCU (เฉพาะอันนี้อันเดียว)
             ------------------------------------------------------*/
            Port1.Write(Encoding.ASCII.GetBytes(":@96331733933349c946f0d178ada618e0\r\n"), 250/*ms*/); // WRITE SOMETHING. ONLY MCU!
            /* -----------------------------------------------
             * รอฟัง...
             * มีข้อความกลับมาหรือไม่...
             -----------------------------------------------*/
            System.Threading.Thread.Sleep(1000); // -> WAIT FOR RESPONSING
            var len1 = Port1.Read(Buffer1, 200/*ms*/); // -> READ SOMETHING.

            /* -----------------------------------------------
             * เช็คว่ามีข้อความกลับมาแล้วหรือไม่...
             * ถ้าไม่มีข้อความกลับ อาจเป็น Indicator
             -----------------------------------------------*/
            if (len1 <= 0)
            {
                try
                {
                    SerialIoManager1.Close();
                }
                catch { }
                try
                {
                    SerialIoManager2.Close();
                }
                catch { }
                Log.Debug("PPX", $"Read data len = {len1}");
                LastResult = -4; LastResultMessage = "UARTS response timeout when seeking MCU!";
                return new int[] { -1, -1 };
            }

            /* -----------------------------------------------
             * แสดง log
             -----------------------------------------------*/
            text2 = $"Found an device. \r\n" +
                  $"MFN: {Port1.Driver.Device.ManufacturerName}, \r\n" +
                  $"VID: {Port1.Driver.Device.VendorId}, \r\n" +
                  $"PID: {Port1.Driver.Device.ProductId}, \r\n" +
                  $"DID: {Port1.Driver.Device.DeviceId}, \r\n" +
                  $"Name: {Port1.Driver.Device.DeviceName}";
            Log.Debug("PPX", text2);

            /* -----------------------------------------------
             * สรุปข้อความที่เจอ
             -----------------------------------------------*/
            var data1 = new byte[len1];
            Array.Copy(Buffer1, data1, len1);
            string converted1 = Encoding.UTF8.GetString(data1, 0, data1.Length);
            Log.Debug("PPX", converted1);
            if (converted1.ToUpper().Contains("PAPER-X"))
            {
                SerialIoManager1.Close();
                Log.Debug("PPX", $"Found MCU on device {iDev}.");
                McuFoundFlag = true;
                McuDeviceIndex = iDev;
            }
            else if (converted1.Contains("kg"))
            {
                SerialIoManager1.Close();
                Log.Debug("PPX", $"Found indicator on device {iDev}.");
                IndicatorFoundFlag = true;
                IndicatorDeviceIndex = iDev;
            }
            else
            {
                SerialIoManager1.Close();
                Log.Debug("PPX", $"Not found but close.");
            }

            /* -----------------------------------------------
             * a. เจอ MCU แล้วหา Indicator ต่อไป
             * b. เจอ Indicator แล้วหา MCU ต่อไป
             -----------------------------------------------*/
            if (McuFoundFlag && !IndicatorFoundFlag) //----------------------------------------------- Second seek ----------------------------------------
            {
                iDev++;
                Port2 = drivers[iDev].Ports[0]; // dev-> next: indicator.
                SerialIoManager2 = new SerialInputOutputManager(Port2)
                {
                    BaudRate = 9600,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                };
                Log.Info("PPX", "Starting INDICATOR(Focus) IO manager...");
                var permissionGranted2 = await usbManager.RequestPermissionAsync(Port2.Driver.Device, c);
                if (!permissionGranted2)
                {
                    try
                    {
                        SerialIoManager1.Close();
                    }
                    catch { }
                    try
                    {
                        SerialIoManager2.Close();
                    }
                    catch { }
                    Log.Info("PPX", "Indicator permission denined!");
                    LastResult = -5; LastResultMessage = "Indicator USB permission denined!";
                    return new int[] { -1, -1 };
                }

                /* ------------------------------------------------------
                 * เปิดพอร์ต เริ่มจาก MCU
                 ------------------------------------------------------*/
                Buffer2 = new byte[DEFAULT_BUFFERSIZE];
                SerialIoManager2.Open(usbManager); // -> OPEN
                System.Threading.Thread.Sleep(1000); // -> WAIT FOR HARDWARE RESET VIA UARTS

                /* -----------------------------------------------
                 * รอฟัง...
                 * มีข้อความกลับมาหรือไม่...
                 -----------------------------------------------*/
                var len2 = Port2.Read(Buffer2, 200/*ms*/); // -> READ SOMETHING.

                /* -----------------------------------------------
                 * เช็คว่ามีข้อความกลับมาแล้วหรือไม่...
                 * ถ้าไม่มีข้อความกลับ อาจเป็น Indicator
                 -----------------------------------------------*/
                if (len2 <= 0)
                {
                    try
                    {
                        SerialIoManager1.Close();
                    }
                    catch { }
                    try
                    {
                        SerialIoManager2.Close();
                    }
                    catch { }
                    Log.Debug("PPX", $"Read data len = {len2}");
                    LastResult = -4; LastResultMessage = "UARTS response timeout when seeking INDICATOR!";
                    return new int[] { -1, -1 };
                }

                /* -----------------------------------------------
                 * แสดง log
                 -----------------------------------------------*/
                text2 = $"Found a device on port 2. \r\n" +
                      $"MFN: {Port2.Driver.Device.ManufacturerName}, \r\n" +
                      $"VID: {Port2.Driver.Device.VendorId}, \r\n" +
                      $"PID: {Port2.Driver.Device.ProductId}, \r\n" +
                      $"DID: {Port2.Driver.Device.DeviceId}, \r\n" +
                      $"Name: {Port2.Driver.Device.DeviceName}";
                Log.Debug("PPX", text2);

                /* -----------------------------------------------
                 * สรุปข้อความที่เจอ
                 -----------------------------------------------*/
                var data2 = new byte[len2];
                Array.Copy(Buffer2, data2, len2);
                string converted2 = Encoding.UTF8.GetString(data2, 0, data2.Length);
                Log.Debug("PPX", converted2);
                if (converted2.Contains("kg"))
                {
                    SerialIoManager2.Close();
                    Log.Debug("PPX", $"Found Indicator on device {iDev}.");
                    IndicatorFoundFlag = true;
                    IndicatorDeviceIndex = iDev;
                }
                else
                {
                    SerialIoManager2.Close();
                    Log.Debug("PPX", $"Not found but close.");
                }
            }
            else if (IndicatorFoundFlag && !McuFoundFlag) //----------------------------------------------- Second seek ----------------------------------------
            {
                iDev++;
                Port2 = drivers[iDev].Ports[0]; // dev-> next: indicator.
                SerialIoManager2 = new SerialInputOutputManager(Port2)
                {
                    BaudRate = 9600,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                };
                Log.Info("PPX", "Starting MCU(Focus) IO manager...");
                var permissionGranted2 = await usbManager.RequestPermissionAsync(Port2.Driver.Device, c);
                if (!permissionGranted2)
                {
                    try
                    {
                        SerialIoManager1.Close();
                    }
                    catch { }
                    try
                    {
                        SerialIoManager2.Close();
                    }
                    catch { }
                    Log.Info("PPX", "Indicator(Focus) permission denined!");
                    LastResult = -5; LastResultMessage = "Indicator(Focus) USB permission denined!";
                    return new int[] { -1, -1 };
                }

                /* ------------------------------------------------------
                 * เปิดพอร์ต ต่อไป เริ่มจาก MCU
                 ------------------------------------------------------*/
                Buffer2 = new byte[DEFAULT_BUFFERSIZE];
                SerialIoManager2.Open(usbManager); // -> OPEN
                System.Threading.Thread.Sleep(100); // -> WAIT FOR HARDWARE RESET VIA UARTS

                /* ------------------------------------------------------
                 * ส่งข้อความถึง MCU (เฉพาะอันนี้อันเดียว)
                 ------------------------------------------------------*/
                Port2.Write(Encoding.ASCII.GetBytes(":@96331733933349c946f0d178ada618e0\r\n"), 250/*ms*/); // WRITE SOMETHING. ONLY MCU! UNLOCK_KEY "@96331733933349c946f0d178ada618e0" // ENERGETIC-PAPERX2-042020

                /* -----------------------------------------------
                 * รอฟัง...
                 * มีข้อความกลับมาหรือไม่...
                 -----------------------------------------------*/
                System.Threading.Thread.Sleep(1000);
                var len2 = Port2.Read(Buffer2, 200/*ms*/); // -> READ SOMETHING.

                /* -----------------------------------------------
                 * เช็คว่ามีข้อความกลับมาแล้วหรือไม่...
                 * ถ้าไม่มีข้อความกลับ อาจเป็น Indicator
                 -----------------------------------------------*/
                if (len2 <= 0)
                {
                    try
                    {
                        SerialIoManager1.Close();
                    }
                    catch { }
                    try
                    {
                        SerialIoManager2.Close();
                    }
                    catch { }
                    Log.Debug("PPX", $"Read data len = {len2}");
                    LastResult = -4; LastResultMessage = "UARTS response timeout when seeking MCU(Focus)!";
                    return new int[] { -1, -1 };
                }

                /* -----------------------------------------------
                 * แสดง log
                 -----------------------------------------------*/
                text2 = $"Found a device on port 2. \r\n" +
                       $"MFN: {Port2.Driver.Device.ManufacturerName}, \r\n" +
                       $"VID: {Port2.Driver.Device.VendorId}, \r\n" +
                       $"PID: {Port2.Driver.Device.ProductId}, \r\n" +
                       $"DID: {Port2.Driver.Device.DeviceId}, \r\n" +
                       $"Name: {Port2.Driver.Device.DeviceName}";
                Log.Debug("PPX", text2);

                /* -----------------------------------------------
                 * สรุปข้อความที่เจอ
                 -----------------------------------------------*/
                var data2 = new byte[len2];
                Array.Copy(Buffer2, data2, len2);
                string converted2 = Encoding.UTF8.GetString(data2, 0, data2.Length);
                Log.Debug("PPX", converted2);
                if (converted2.ToUpper().Contains("PAPER-X"))
                {
                    SerialIoManager2.Close();
                    Log.Debug("PPX", $"Found MCU(focus) on device {iDev}.");
                    McuFoundFlag = true;
                    McuDeviceIndex = iDev;
                }
                else
                {
                    SerialIoManager2.Close();
                    Log.Debug("PPX", $"Not found but close.");
                }
            }
            else
            {
                try
                {
                    SerialIoManager1.Close();
                }
                catch { }
                try
                {
                    SerialIoManager2.Close();
                }
                catch { }
                LastResult = -6; LastResultMessage = "เชื่อมต่อพอร์ตได้ แต่ไม่ใช่อุปกรณ์พ่วงทึ่ต้องการค้นหา";
                return new int[] { -1, -1 };
            }

            /* ----------------------------------------------------------------
             * Summarize
             ----------------------------------------------------------------*/
            if (McuFoundFlag && IndicatorFoundFlag)
            {
                LastResult = 0; LastResultMessage = $"พบอุปกรณ์ครบทั้ง 2 ตัว\r\n{text1}\r\n\r\n{text2}";
                return new int[] { McuDeviceIndex, IndicatorDeviceIndex };
            }
            else
            {
                try
                {
                    SerialIoManager1.Close();
                }
                catch { }
                try
                {
                    SerialIoManager2.Close();
                }
                catch { }
                LastResult = -11; LastResultMessage = "ไม่ได้จบคำสั่งอย่างปกติ แต่ทำงานมาจนถึงบรรทัดสุดท้าย";
                return new int[] { -1, -1 };
            }
        }
        catch (Exception ex)
        {
            try
            {
                SerialIoManager1.Close();
            }
            catch { }
            try
            {
                SerialIoManager2.Close();
            }
            catch { }
            Log.Debug("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
            LastResult = -90; LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
            return new int[] { -1, -1 };
        }
    }
    public async Task<int> McuSeek(int Timeout = 2000)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usb1anager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            for (int idev = 0; idev < drivers.Count; idev++)
            {
                Port1 = drivers[idev].Ports[0];

                SerialIoManager1 = new SerialInputOutputManager(Port1)
                {
                    BaudRate = 9600,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                };

                Log.Info("PPX", "Starting IO manager ..");
                var permissionGranted = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
                if (permissionGranted)
                {
                    //------------------------------------------------------
                    // SEEK MCU
                    //------------------------------------------------------
                    try
                    {
                        McuBuffer = new byte[DEFAULT_BUFFERSIZE];

                        SerialIoManager1.Open(usbManager);
                        System.Threading.Thread.Sleep(100);

                        Port1.Write(Encoding.ASCII.GetBytes(":@96331733933349c946f0d178ada618e0\r\n"), 70);

                        long maxReadTime = Timeout;
                        long t0_ms = DateTime.Now.Ticks / 10000;
                        StopFlag = false;
                        bool FoundFlag = false;
                        while (!StopFlag)
                        {
                            //-----------------------------------------------
                            // handle incoming data.
                            //-----------------------------------------------
                            System.Threading.Thread.Sleep(100);
                            var len = Port1.Read(McuBuffer, READ_1_WAIT_MILLIS);
                            if (len > 0)
                            {
                                Log.Debug("PPX", $"Read data len = {len}");

                                var data = new byte[len];
                                Array.Copy(McuBuffer, data, len);

                                string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                                Log.Debug("PPX", converted);

                                if (converted.Contains("P"))
                                {
                                    SerialIoManager1.Close();
                                    Log.Debug("PPX", $"Found MCU on device {idev}.");
                                    FoundFlag = true;
                                    StopFlag = true;
                                    break;
                                }
                            }

                            //-----------------------------------------------
                            // TIMEOUT
                            //-----------------------------------------------
                            long current_t_ms = DateTime.Now.Ticks / 10000;
                            if (maxReadTime < current_t_ms - t0_ms)
                            {
                                SerialIoManager1.Close();
                                Log.Debug("PPX", "Read Timeout!");
                                StopFlag = true;
                            }
                        }
                        // ---------------------------------------------
                        // Return;
                        // ---------------------------------------------
                        if (FoundFlag)
                        {
                            return idev;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (SerialIoManager1 != null) SerialIoManager1.Close();
                        Log.Info("PPX", ex.Message);
                        return -4;
                    }
                }
                else
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", "Permission denined!");
                    return -3;
                }
            }
            // --------------------------------------------
            // Safety return
            // --------------------------------------------
            return -2;
        }
        catch (Java.IO.IOException e)
        {
            Log.Info("PPX", e.Message);
            return -1;
        }
    }

    public async Task<int> IndicatorSeek(int Timeout = 2000)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            for (int idev = 0; idev < drivers.Count; idev++)
            {
                Port2 = drivers[idev].Ports[0];

                SerialIoManager2 = new SerialInputOutputManager(Port2)
                {
                    BaudRate = 9600,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                };

                Log.Info("PPX", "Starting Indicator IO manager ..");
                var permissionGranted = await usbManager.RequestPermissionAsync(Port2.Driver.Device, c);
                if (permissionGranted)
                {
                    //------------------------------------------------------
                    // SEEK MCU
                    //------------------------------------------------------
                    try
                    {
                        IndicatorBuffer = new byte[DEFAULT_BUFFERSIZE];

                        SerialIoManager2.Open(usbManager);
                        System.Threading.Thread.Sleep(100);

                        long maxReadTime = Timeout;
                        long t0_ms = DateTime.Now.Ticks / 10000;
                        bool FoundFlag = false;
                        StopFlag = false;
                        while (!StopFlag)
                        {
                            //-----------------------------------------------
                            // handle incoming data.
                            //-----------------------------------------------
                            var len = Port2.Read(IndicatorBuffer, READ_1_WAIT_MILLIS);
                            if (len > 0)
                            {
                                Log.Debug("PPX", $"Read data len = {len}");

                                var data = new byte[len];
                                Array.Copy(IndicatorBuffer, data, len);

                                string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                                Log.Debug("PPX", converted);

                                if (converted.Contains('\x02') || converted.Contains('\x03'))
                                {
                                    SerialIoManager2.Close();
                                    Log.Debug("PPX", $"Found Indicator on device {idev}.");
                                    FoundFlag = true;
                                    StopFlag = true;
                                    break;
                                }
                            }

                            //-----------------------------------------------
                            // TIMEOUT
                            //-----------------------------------------------
                            long current_t_ms = DateTime.Now.Ticks / 10000;
                            if (maxReadTime < current_t_ms - t0_ms)
                            {
                                SerialIoManager2.Close();
                                Log.Debug("PPX", "Read Timeout!");
                                StopFlag = true;
                            }
                        }
                        // ---------------------------------------------
                        // Return;
                        // ---------------------------------------------
                        if (FoundFlag)
                        {
                            return idev;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (SerialIoManager1 != null) SerialIoManager1.Close();
                        Log.Info("PPX", ex.Message);
                        return -4;
                    }
                }
                else
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", "Permission denined!");
                    return -3;
                }
            }
            // --------------------------------------------
            // Safety return
            // --------------------------------------------
            return -2;
        }
        catch (Java.IO.IOException e)
        {
            Log.Info("PPX", e.Message);
            return -1;
        }
    }

    public async Task<string> HomeShutter(int index, long waitTime, Label label)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            Port1 = drivers[index].Ports[0];

            SerialIoManager1 = new SerialInputOutputManager(Port1)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };

            Log.Info("PPX", "MCU: Starting IO manager ..");
            var permissionGranted = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
            if (permissionGranted)
            {
                try
                {
                    McuBuffer = new byte[DEFAULT_BUFFERSIZE];
                    SerialIoManager1.Open(usbManager);
                    System.Threading.Thread.Sleep(100);

                    Port1.Write(new byte[] { (byte)':', (byte)'d', (byte)'0', 0x0D, 0x0A }, 70);

                    // * ack
                    StopFlag = false;
                    Stopwatch swLittenHomeShutter = new Stopwatch();
                    swLittenHomeShutter.Start();
                    while (!StopFlag)
                    {
                        //-----------------------------------------------
                        // handle incoming data.
                        //-----------------------------------------------
                        var len = Port1.Read(McuBuffer, READ_1_WAIT_MILLIS);
                        if (len > 0)
                        {
                            Log.Debug("PPX", $"Read data len = {len}");

                            var data = new byte[len];
                            Array.Copy(McuBuffer, data, len);

                            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                            Log.Debug("PPX", converted);

                            //if (converted.Contains("#"))
                            if (converted.Contains("=") || converted.Contains("*0"))
                            {
                                swLittenHomeShutter.Stop();
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"Shutter homing done. ({swLittenHomeShutter.ElapsedMilliseconds} ms).");
                                LastResult = 0;
                                LastResultMessage = $"Shutter homing done. ({swLittenHomeShutter.ElapsedMilliseconds} ms).";
                                return "00";
                            }

                            if (converted.Contains("!"))
                            {
                                swLittenHomeShutter.Stop();
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"HomeShutter() - Hardware timeout! ({swLittenHomeShutter.ElapsedMilliseconds} ms).");
                                LastResult = 1;
                                LastResultMessage = $"HomeShutter() - Hardware timeout! ({swLittenHomeShutter.ElapsedMilliseconds} ms).";
                                return "01";
                            }
                        }

                        //-----------------------------------------------
                        // TIMEOUT
                        //-----------------------------------------------
                        System.Threading.Thread.Sleep(100);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (label != null) label.Text = $"{swLittenHomeShutter.ElapsedMilliseconds / 1000} sec";
                        });

                        if (waitTime < swLittenHomeShutter.ElapsedMilliseconds)
                        {
                            SerialIoManager1.Close();
                            Log.Debug("PPX", "HomeShutter() - Software timeout!");
                            LastResult = 2;
                            LastResultMessage = "HomeShutter() - Software timeout!";
                            return "02";
                        }
                    }//

                    LastResult = 3;
                    LastResultMessage = "OpenDoor() - Software jump out! No any catch.";
                    return "03";
                }
                catch (Exception ex)
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
                    LastResult = 4;
                    LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
                    return $"04:{ex.Message}\r\n{ex.StackTrace}";
                }
            }
            else
            {
                if (SerialIoManager1 != null) SerialIoManager1.Close();
                Log.Info("PPX", "Permission denined!");
                LastResult = 5;
                LastResultMessage = $"Permission denined!";
                return "05:Denined!";
            }
        }
        catch (Java.IO.IOException ex)
        {
            Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
            LastResult = 6;
            LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}"; ;
            return $"{ex.Message}\r\n{ex.StackTrace}"; ;
        }
    }

    public async Task<string> OpenShutter(int index, long waitTime, Label label)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            Port1 = drivers[index].Ports[0];

            SerialIoManager1 = new SerialInputOutputManager(Port1)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };

            Log.Info("PPX", "OpenShutter(): Starting IO manager ..");
            var permissionGranted = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
            if (permissionGranted)
            {
                try
                {
                    McuBuffer = new byte[DEFAULT_BUFFERSIZE];
                    SerialIoManager1.Open(usbManager);
                    System.Threading.Thread.Sleep(100);

                    Port1.Write(new byte[] { (byte)':', (byte)'d', (byte)'1', 0x0D, 0x0A }, 70);

                    // * ack
                    StopFlag = false;
                    Stopwatch swOpenShutter = new Stopwatch();
                    swOpenShutter.Start();
                    while (!StopFlag)
                    {
                        //-----------------------------------------------
                        // handle incoming data.
                        //-----------------------------------------------
                        var len = Port1.Read(McuBuffer, READ_1_WAIT_MILLIS);
                        if (len > 0)
                        {
                            Log.Debug("PPX", $"Read data len = {len}");

                            var data = new byte[len];
                            Array.Copy(McuBuffer, data, len);

                            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                            Log.Debug("PPX", converted);

                            //if (converted.Contains("#"))
                            if (converted.Contains("=") || converted.Contains("*0"))
                            {
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"Shutter opened ({swOpenShutter.ElapsedMilliseconds} ms).");
                                LastResult = 0;
                                LastResultMessage = $"Done shutter opening ({swOpenShutter.ElapsedMilliseconds} ms).";
                                return "00";
                            }

                            if (converted.Contains("!"))
                            {
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"OpenShutter() - Hardware timeout! ({swOpenShutter.ElapsedMilliseconds} ms).");
                                LastResult = 1;
                                LastResultMessage = $"OpenShutter() - Hardware timeout! ({swOpenShutter.ElapsedMilliseconds} ms).";
                                return "01";
                            }
                        }

                        //-----------------------------------------------
                        // TIMEOUT
                        //-----------------------------------------------
                        System.Threading.Thread.Sleep(100);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (label != null) label.Text = $"{(swOpenShutter.ElapsedMilliseconds) / 1000} s";
                        });

                        if (waitTime < swOpenShutter.ElapsedMilliseconds)
                        {
                            SerialIoManager1.Close();
                            Log.Debug("PPX", "OpenDoor() - Software timeout!");
                            LastResult = 2;
                            LastResultMessage = "OpenShutter() - Software timeout!";
                            return "02";
                        }
                    }//

                    LastResult = 3;
                    LastResultMessage = "OpenShutter() - Jump out by no catch!";
                    return "03";
                }
                catch (Exception ex)
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
                    LastResult = 4;
                    LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
                    return $"{ex.Message}\r\n{ex.StackTrace}";
                }
            }
            else
            {
                if (SerialIoManager1 != null) SerialIoManager1.Close();
                Log.Info("PPX", "Permission denined!");
                LastResult = 5;
                LastResultMessage = "Permission denined!";
                return "05:Denined!";
            }
        }
        catch (Java.IO.IOException ex)
        {
            Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
            LastResult = 6;
            LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
            return $"06:{ex.Message}\r\n{ex.StackTrace}";
        }
    }

    public async Task<string> CloseShutter(int index, long waitTime, Label label)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            Port1 = drivers[index].Ports[0];

            SerialIoManager1 = new SerialInputOutputManager(Port1)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };

            Log.Info("PPX", "CloseShutter(): Starting IO manager ..");
            var permissionGranted = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
            if (permissionGranted)
            {
                try
                {
                    McuBuffer = new byte[DEFAULT_BUFFERSIZE];
                    SerialIoManager1.Open(usbManager);
                    System.Threading.Thread.Sleep(100);

                    Port1.Write(new byte[] { (byte)':', (byte)'d', (byte)'2', 0x0D, 0x0A }, 70);

                    // * ack
                    StopFlag = false;
                    Stopwatch swCloseShutter = new Stopwatch();
                    swCloseShutter.Start();
                    while (!StopFlag)
                    {
                        //-----------------------------------------------
                        // handle incoming data.
                        //-----------------------------------------------
                        var len = Port1.Read(McuBuffer, READ_1_WAIT_MILLIS);
                        if (len > 0)
                        {
                            Log.Debug("PPX", $"Read data len = {len}");

                            var data = new byte[len];
                            Array.Copy(McuBuffer, data, len);

                            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                            Log.Debug("PPX", converted);

                            if (converted.Contains("=") || converted.Contains("0\r\n"))
                            {
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"Shutter closed ({swCloseShutter.ElapsedMilliseconds} ms).");
                                LastResult = 0;
                                LastResultMessage = $"Shutter closed ({swCloseShutter.ElapsedMilliseconds} ms).";
                                return "00";
                            }

                            if (converted.Contains("!"))
                            {
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"CloseShutter() - Hardware timeout! ({swCloseShutter.ElapsedMilliseconds} ms).");
                                LastResult = 1;
                                LastResultMessage = $"CloseShutter() - Hardware timeout! ({swCloseShutter.ElapsedMilliseconds} ms).";
                                return "01";
                            }
                        }

                        //-----------------------------------------------
                        // TIMEOUT
                        //-----------------------------------------------
                        if (waitTime < swCloseShutter.ElapsedMilliseconds)
                        {
                            SerialIoManager1.Close();
                            Log.Debug("PPX", "CloseShutter() - Software timeout!");
                            LastResult = 2;
                            LastResultMessage = "CloseShutter() - Software timeout!";
                            return "02";
                        }
                    }
                    //
                    LastResult = 3;
                    LastResultMessage = "CloseShutter() - Jump out by no catch.";
                    return "03";
                }
                catch (Exception ex)
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
                    LastResult = 4;
                    LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
                    return $"04:{ex.Message}\r\n{ex.StackTrace}";
                }
            }
            else
            {
                if (SerialIoManager1 != null) SerialIoManager1.Close();
                Log.Info("PPX", "Permission denined!");
                LastResult = 5;
                LastResultMessage = "Permission denined!";
                return "05:Denined!";
            }
        }
        catch (Java.IO.IOException ex)
        {
            Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
            LastResult = 6;
            LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
            return $"06:{ex.Message}\r\n{ex.StackTrace}";
        }
    }

    public async Task<string> Kick(int index, long waitTime, Label label)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            Port1 = drivers[index].Ports[0];

            SerialIoManager1 = new SerialInputOutputManager(Port1)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };

            Log.Info("PPX", "MCU: Starting IO manager ..");
            var permissionGranted = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
            if (permissionGranted)
            {
                try
                {
                    McuBuffer = new byte[DEFAULT_BUFFERSIZE];
                    SerialIoManager1.Open(usbManager);
                    System.Threading.Thread.Sleep(100);

                    // supot : 2020-02-19 เปลี่ยน 1 เป็น 8
                    // Port1.Write(new byte[] { (byte)'$', (byte)'k', (byte)'1', (byte)'#', }, 70);
                    Port1.Write(new byte[] { (byte)'$', (byte)'k', (byte)'8', (byte)'#', }, 70);

                    // * ack
                    StopFlag = false;
                    Stopwatch swKick = new Stopwatch();
                    swKick.Start();
                    while (!StopFlag)
                    {
                        //-----------------------------------------------
                        // handle incoming data.
                        //-----------------------------------------------
                        var len = Port1.Read(McuBuffer, READ_1_WAIT_MILLIS);
                        if (len > 0)
                        {
                            Log.Debug("PPX", $"Read data len = {len}");

                            var data = new byte[len];
                            Array.Copy(McuBuffer, data, len);

                            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                            Log.Debug("PPX", converted);

                            if (converted.Contains("=") || converted.Contains("*0"))
                            {
                                swKick.Stop();
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"Kicked ({swKick.ElapsedMilliseconds} ms).");
                                LastResult = 0;
                                LastResultMessage = $"Kicked ({swKick.ElapsedMilliseconds} ms).";
                                return "00";
                            }

                            if (converted.Contains("!"))
                            {
                                swKick.Stop();
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"Kick() - Hardware timeout! ({swKick.ElapsedMilliseconds} ms).");
                                LastResult = 1;
                                LastResultMessage = $"Kick() - Hardware timeout! ({swKick.ElapsedMilliseconds} ms).";
                                return "01";
                            }
                        }

                        //-----------------------------------------------
                        // TIMEOUT
                        //-----------------------------------------------
                        System.Threading.Thread.Sleep(100);
                        if (waitTime < swKick.ElapsedMilliseconds)
                        {
                            swKick.Stop();
                            SerialIoManager1.Close();
                            Log.Debug("PPX", "Kick() - Software timeout!");
                            LastResult = 2;
                            LastResultMessage = "Kick() - Software timeout!";
                            return "02";
                        }
                    }

                    LastResult = 3;
                    LastResultMessage = "Kick() - Jump out by no catch!";
                    return "03";

                }
                catch (Exception ex)
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
                    LastResult = 4;
                    LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
                    return $"04:{ex.Message}\r\n{ex.StackTrace}";
                }
            }
            else
            {
                if (SerialIoManager1 != null) SerialIoManager1.Close();
                Log.Info("PPX", "Permission denined!");
                LastResult = 5;
                LastResultMessage = "Permission denined!";
                return "05:Permission denined!";
            }
        }
        catch (Java.IO.IOException ex)
        {
            Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
            LastResult = 6;
            LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
            return $"06:{ex.Message}\r\n{ex.StackTrace}";
        }
    }

    public async Task<string> HomeKicker(int index, long waitTime, Label label)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            Port1 = drivers[index].Ports[0];

            SerialIoManager1 = new SerialInputOutputManager(Port1)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };

            Log.Info("PPX", "MCU: Starting IO manager ..");
            var permissionGranted = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
            if (permissionGranted)
            {
                try
                {
                    McuBuffer = new byte[DEFAULT_BUFFERSIZE];
                    SerialIoManager1.Open(usbManager);
                    System.Threading.Thread.Sleep(100);

                    Port1.Write(new byte[] { (byte)'$', (byte)'k', (byte)'0', (byte)'#', }, 70);

                    // * ack
                    StopFlag = false;
                    Stopwatch swKick = new Stopwatch();
                    swKick.Start();
                    while (!StopFlag)
                    {
                        //-----------------------------------------------
                        // handle incoming data.
                        //-----------------------------------------------
                        var len = Port1.Read(McuBuffer, READ_1_WAIT_MILLIS);
                        if (len > 0)
                        {
                            Log.Debug("PPX", $"Read data len = {len}");

                            var data = new byte[len];
                            Array.Copy(McuBuffer, data, len);

                            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                            Log.Debug("PPX", converted);

                            if (converted.Contains("=") || converted.Contains("*0"))
                            {
                                swKick.Stop();
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"Kicked ({swKick.ElapsedMilliseconds} ms).");
                                LastResult = 0;
                                LastResultMessage = $"Kicked ({swKick.ElapsedMilliseconds} ms).";
                                return "00";
                            }

                            if (converted.Contains("!"))
                            {
                                swKick.Stop();
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"Kick() - Hardware timeout! ({swKick.ElapsedMilliseconds} ms).");
                                LastResult = 1;
                                LastResultMessage = $"Kick() - Hardware timeout! ({swKick.ElapsedMilliseconds} ms).";
                                return "01";
                            }
                        }

                        //-----------------------------------------------
                        // TIMEOUT
                        //-----------------------------------------------
                        System.Threading.Thread.Sleep(100);
                        if (waitTime < swKick.ElapsedMilliseconds)
                        {
                            swKick.Stop();
                            SerialIoManager1.Close();
                            Log.Debug("PPX", "Kick() - Software timeout!");
                            LastResult = 2;
                            LastResultMessage = "Kick() - Software timeout!";
                            return "02";
                        }
                    }

                    LastResult = 3;
                    LastResultMessage = "Kick() - Jump out by no catch!";
                    return "03";

                }
                catch (Exception ex)
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
                    LastResult = 4;
                    LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
                    return $"04:{ex.Message}\r\n{ex.StackTrace}";
                }
            }
            else
            {
                if (SerialIoManager1 != null) SerialIoManager1.Close();
                Log.Info("PPX", "Permission denined!");
                LastResult = 5;
                LastResultMessage = "Permission denined!";
                return "05:Permission denined!";
            }
        }
        catch (Java.IO.IOException ex)
        {
            Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
            LastResult = 6;
            LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
            return $"06:{ex.Message}\r\n{ex.StackTrace}";
        }
    }

    public async Task<string> Weighting(int index, long waitTime)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            Port2 = drivers[index].Ports[0];

            SerialIoManager2 = new SerialInputOutputManager(Port2)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };

            Log.Info("PPX", "Indicator: Starting IO manager ..");
            var permissionGranted = await usbManager.RequestPermissionAsync(Port2.Driver.Device, c);
            if (permissionGranted)
            {
                try
                {
                    IndicatorBuffer = new byte[DEFAULT_BUFFERSIZE];
                    SerialIoManager2.Open(usbManager);
                    System.Threading.Thread.Sleep(200);

                    StopFlag = false;
                    Stopwatch swIndicator = new Stopwatch();
                    swIndicator.Start();
                    while (!StopFlag)
                    {
                                              

                        //-----------------------------------------------
                        // handle incoming data.
                        //-----------------------------------------------
                        var len = Port2.Read(IndicatorBuffer, READ_2_WAIT_MILLIS);
                        if (len > 0)
                        {
                            //Log.Debug("PPX", $"Read data len = {len}");

                            var data = new byte[len];
                            Array.Copy(IndicatorBuffer, data, len);

                            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                            //Log.Debug("PPX", converted);

                            if (converted.Contains("\r\n"))
                            {
                                // Retrieve value from array
                                Log.Debug("PPX", converted);
                                string[] lines = converted.Split("\r\n");
                                string sWeight = "+000000"; //gram
                                double[] w_array = new double[lines.Length];
                                double sumW = 0;
                                int idxW = 0;
                                foreach (string s in lines)
                                {
                                    if ((s.Contains("US") || s.Contains("ST")) && (s.Contains("kg")))
                                    {
                                        if (s.Contains("ST"))
                                        {
                                            try
                                            {
                                                w_array[idxW] = double.Parse(s.Substring(6, 8).Replace(" ", ""));
                                                sumW += w_array[idxW];
                                                idxW++;
                                            }
                                            catch { }
                                        }
                                    }
                                }

                                if (idxW <= 0) continue;

                                double avgW = sumW / idxW;

                                sWeight = avgW.ToString();

                                // Weight OK
                                swIndicator.Stop();
                                SerialIoManager2.Close();
                                Log.Debug("PPX", $"Weight {sWeight}.");
                                StopFlag = true;
                                LastResult = 0;
                                LastResultMessage = $"Weight = {sWeight} g.";
                                return $"00:{sWeight}";
                            }
                        }

                        //-----------------------------------------------
                        // TIMEOUT
                        //-----------------------------------------------
                        System.Threading.Thread.Sleep(100);
                        if (waitTime < swIndicator.ElapsedMilliseconds)
                        {
                            SerialIoManager2.Close();
                            Log.Debug("PPX", "Weighting Read Timeout!");
                            StopFlag = true;
                            LastResult = 2;
                            LastResultMessage = "Weight Reading Timeout!";
                            return "02:Read Timeout!";
                        }
                    }

                    LastResult = 3;
                    LastResultMessage = "HomeShutter() - Software jump out! No any catch.";
                    return "03";
                }
                catch (Exception ex)
                {
                    if (SerialIoManager2 != null) SerialIoManager2.Close();
                    Log.Info("PPX", ex.Message);
                    LastResult = 4;
                    LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
                    return $"04:{ex.Message}\r\n{ex.StackTrace}";
                }
            }
            else
            {
                if (SerialIoManager2 != null) SerialIoManager2.Close();
                Log.Info("PPX", "Permission denined!");
                LastResult = 5;
                LastResultMessage = "Permission denined!";
                return $"05:Denined!";
            }
        }
        catch (Java.IO.IOException ex)
        {
            Log.Info("PPX", ex.Message);
            LastResult = 6;
            LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
            return $"06:{ex.Message}\r\n{ex.StackTrace}";
        }
    }

    public async Task<string> ReadSensor(int iSen, int portIndex, long waitTime)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            Port1 = drivers[portIndex].Ports[0];

            SerialIoManager1 = new SerialInputOutputManager(Port1)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };

            Log.Info("PPX", "MCU: Starting IO manager ..");
            var permissionGranted = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
            if (permissionGranted)
            {
                try
                {
                    McuBuffer = new byte[DEFAULT_BUFFERSIZE];
                    SerialIoManager1.Open(usbManager);
                    System.Threading.Thread.Sleep(100);

                    Port1.Write(new byte[] { (byte)'$', (byte)'s', (byte)('0' + iSen), (byte)'#', }, 70);

                    // * ack
                    StopFlag = false;
                    Stopwatch swSensor = new Stopwatch();
                    swSensor.Start();
                    while (!StopFlag)
                    {
                        //-----------------------------------------------
                        // handle incoming data.
                        //-----------------------------------------------
                        var len = Port1.Read(McuBuffer, READ_1_WAIT_MILLIS);
                        if (len > 0)
                        {
                            Log.Debug("PPX", $"Read data len = {len}");

                            var data = new byte[len];
                            Array.Copy(McuBuffer, data, len);

                            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                            Log.Debug("PPX", converted);

                            if (converted.Contains("="))
                            {
                                swSensor.Stop();
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"Sensor-{iSen} read ({swSensor.ElapsedMilliseconds} ms).");
                                LastResult = 0;
                                LastResultMessage = $"Sensor-{iSen} read:{converted} ({swSensor.ElapsedMilliseconds} ms).";
                                return $"00:{converted.Replace("=", "")}";
                            }
                        }

                        //-----------------------------------------------
                        // TIMEOUT
                        //-----------------------------------------------
                        System.Threading.Thread.Sleep(100);
                        if (waitTime < swSensor.ElapsedMilliseconds)
                        {
                            swSensor.Stop();
                            SerialIoManager1.Close();
                            Log.Debug("PPX", $"ReadSensor():{iSen} - Software timeout!");
                            LastResult = 2;
                            LastResultMessage = $"ReadSensor():{iSen} - Software timeout!";
                            return "02";
                        }
                    }

                    LastResult = 3;
                    LastResultMessage = $"ReadSensor():{iSen} - Jump out by no catch!";
                    return "03";

                }
                catch (Exception ex)
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
                    LastResult = 4;
                    LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
                    return $"04:{ex.Message}\r\n{ex.StackTrace}";
                }
            }
            else
            {
                if (SerialIoManager1 != null) SerialIoManager1.Close();
                Log.Info("PPX", "Permission denined!");
                LastResult = 5;
                LastResultMessage = "Permission denined!";
                return "05:Permission denined!";
            }
        }
        catch (Java.IO.IOException ex)
        {
            Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
            LastResult = 6;
            LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
            return $"06:{ex.Message}\r\n{ex.StackTrace}";
        }
    }

    public async Task<string> ReadAllSensor(int portIndex, long waitTime)
    {
        try
        {
            Context c = Forms.Context;
            UsbManager usbManager = (UsbManager)c.GetSystemService(Context.UsbService);
            usbManager = c.GetSystemService(Context.UsbService) as UsbManager;
            if (drivers == null || drivers.Count <= 0)
            {
                drivers = FindAllDrivers(usbManager);
            }
            Port1 = drivers[portIndex].Ports[0];

            SerialIoManager1 = new SerialInputOutputManager(Port1)
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
            };

            Log.Info("PPX", "MCU: Starting IO manager ..");
            var permissionGranted = await usbManager.RequestPermissionAsync(Port1.Driver.Device, c);
            if (permissionGranted)
            {
                try
                {
                    McuBuffer = new byte[DEFAULT_BUFFERSIZE];
                    SerialIoManager1.Open(usbManager);
                    System.Threading.Thread.Sleep(100);

                    Port1.Write(new byte[] { (byte)':', (byte)'a', (byte)('s'), 0x0D, 0x0A }, 70);

                    // * ack
                    StopFlag = false;
                    Stopwatch swSensor = new Stopwatch();
                    swSensor.Start();
                    while (!StopFlag)
                    {
                        //-----------------------------------------------
                        // handle incoming data.
                        //-----------------------------------------------
                        var len = Port1.Read(McuBuffer, READ_1_WAIT_MILLIS);
                        if (len > 0)
                        {
                            Log.Debug("PPX", $"Read data len = {len}");

                            var data = new byte[len];
                            Array.Copy(McuBuffer, data, len);

                            string converted = Encoding.UTF8.GetString(data, 0, data.Length);
                            Log.Debug("PPX", converted);

                            if (converted.Contains("\r\n"))
                            {
                                swSensor.Stop();
                                SerialIoManager1.Close();
                                Log.Debug("PPX", $"All sensor read ({swSensor.ElapsedMilliseconds} ms).");
                                LastResult = 0;
                                LastResultMessage = $"All sensor read:{converted} ({swSensor.ElapsedMilliseconds} ms).";
                                return $"00:{converted.Replace("=", "")}";
                            }
                        }

                        //-----------------------------------------------
                        // TIMEOUT
                        //-----------------------------------------------
                        System.Threading.Thread.Sleep(100);
                        if (waitTime < swSensor.ElapsedMilliseconds)
                        {
                            swSensor.Stop();
                            SerialIoManager1.Close();
                            Log.Debug("PPX", $"ReadAllSensor() - Software timeout!");
                            LastResult = 2;
                            LastResultMessage = $"ReadAllSensor() - Software timeout!";
                            return "02";
                        }
                    }

                    LastResult = 3;
                    LastResultMessage = $"ReadAllSensor() - Jump out by no catch!";
                    return "03";

                }
                catch (Exception ex)
                {
                    if (SerialIoManager1 != null) SerialIoManager1.Close();
                    Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
                    LastResult = 4;
                    LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
                    return $"04:{ex.Message}\r\n{ex.StackTrace}";
                }
            }
            else
            {
                if (SerialIoManager1 != null) SerialIoManager1.Close();
                Log.Info("PPX", "Permission denined!");
                LastResult = 5;
                LastResultMessage = "Permission denined!";
                return "05:Permission denined!";
            }
        }
        catch (Java.IO.IOException ex)
        {
            Log.Info("PPX", $"{ex.Message}\r\n{ex.StackTrace}");
            LastResult = 6;
            LastResultMessage = $"{ex.Message}\r\n{ex.StackTrace}";
            return $"06:{ex.Message}\r\n{ex.StackTrace}";
        }
    }

    internal static async Task<IList<IUsbSerialDriver>> FindAllDriversAsync(UsbManager usbManager)
    {
        // using the default probe table
        // return UsbSerialProber.DefaultProber.FindAllDriversAsync (usbManager);

        // adding a custom driver to the default probe table
        var table = UsbSerialProber.DefaultProbeTable;
        table.AddProduct(0x1b4f, 0x0008, typeof(CdcAcmSerialDriver)); // IOIO OTG

        table.AddProduct(0x09D8, 0x0420, typeof(CdcAcmSerialDriver)); // Elatec TWN4

        var prober = new UsbSerialProber(table);
        return await prober.FindAllDriversAsync(usbManager);
    }

    internal static IList<IUsbSerialDriver> FindAllDrivers(UsbManager usbManager)
    {
        // using the default probe table
        // return UsbSerialProber.DefaultProber.FindAllDriversAsync (usbManager);

        // adding a custom driver to the default probe table
        var table = UsbSerialProber.DefaultProbeTable;
        table.AddProduct(0x1b4f, 0x0008, typeof(CdcAcmSerialDriver)); // IOIO OTG

        table.AddProduct(0x09D8, 0x0420, typeof(CdcAcmSerialDriver)); // Elatec TWN4

        var prober = new UsbSerialProber(table);
        return prober.FindAllDrivers(usbManager);
    }
}
