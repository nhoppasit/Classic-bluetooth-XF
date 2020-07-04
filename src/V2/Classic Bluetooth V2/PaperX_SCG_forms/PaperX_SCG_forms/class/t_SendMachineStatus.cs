using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


public class p_SendMachineStatus
{
    public int machineStatusId { get; set; }
    public string moreDetail { get; set; }

      public string productCapacity { get; set; }

}

public class p_productCapacity
{
    public int productid { get; set; }
    public double percentBinFull { get; set; }
    public double weightBinFull { get; set; }
}
class t_SendMachineStatus
{
    public class Data
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string saveTree { get; set; }
        public version version { get; set; }
    }
    public class version
    {
        public string productVersion { get; set; }
        public string videoVersion { get; set; }
        public string generalDataVersion { get; set; }
        public string partnerRedeemVersion { get; set; }

    }
    public Data data { get; set; }
}
