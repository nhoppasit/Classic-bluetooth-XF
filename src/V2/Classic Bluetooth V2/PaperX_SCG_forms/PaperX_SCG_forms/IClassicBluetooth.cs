using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft;

namespace PaperX_SCG_forms
{
    public class BasicReturn
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public bool Flag { get; set; }
    }

    public interface IClassicBluetooth
    {
        string Check();

        string Connect(string address, string uuid);

        string Disconnect();

        string ReadInfo();

        string BlinkOn(int time);

        string BlinkOff();

    }
}
