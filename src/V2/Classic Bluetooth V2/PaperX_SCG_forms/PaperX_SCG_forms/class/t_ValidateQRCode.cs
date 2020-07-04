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

public class p_ValidateQRCode
{
    public string qrCode { get; set; }
}
class t_ValidateQRCode
{
    public class Data
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public Data data { get; set; }
}
