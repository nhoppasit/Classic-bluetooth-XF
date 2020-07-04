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


    class t_MasterProduct
{
    public class Data
    {
        public bool success { get; set; }
        public string message { get; set; }
public string version { get; set; }

public product[] product { get; set; }
    }
    public class product
    {
        public string id { get; set; }
        public string nameEN { get; set; }
public string nameTH { get; set; }

    }
    public Data data { get; set; }
}
