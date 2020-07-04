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


public class p_Checksum
{
    public string username { get; set; }
    public string password { get; set; }
    public string timestamp { get; set; }
}

public   class t_Checksum
{
    public class Data
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string checksum { get; set; }
    }

    public Data data { get; set; }

}
