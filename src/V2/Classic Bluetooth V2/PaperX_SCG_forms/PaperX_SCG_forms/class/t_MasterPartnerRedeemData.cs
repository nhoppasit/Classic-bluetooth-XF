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


 public   class t_MasterPartnerRedeemData
{
    public class Data
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string[] version { get; set; }
public string[] partnerRedeemImage { get; set; }

    }

   

    public Data data { get; set; }
}
