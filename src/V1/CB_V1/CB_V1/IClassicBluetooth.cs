using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft;

namespace CB_V1
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



    }
}
