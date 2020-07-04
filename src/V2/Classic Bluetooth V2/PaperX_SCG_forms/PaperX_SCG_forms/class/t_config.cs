using System;
using System.Collections.Generic;
using System.Text;


public class  t_config
{
    public class Data
    {
        public string APIServer { get; set; }
        public string MachineName { get; set; }
        public string MachinePassword { get; set; }
        public string TimeCloseDoor { get; set; }
        public string TimePageBack { get; set; }
        public string TimeSendToAPI { get; set; }
        public string QRTransporter { get; set; }
    }

   
    public Data data { get; set; }
}


