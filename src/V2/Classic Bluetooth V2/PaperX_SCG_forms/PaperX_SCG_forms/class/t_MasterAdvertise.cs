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


 public   class t_MasterAdvertise
{
    public class Data
    {
        public bool success { get; set; }
        public string message { get; set; }

        public advertise[] advertise { get; set; }
    }

    public class advertise
    {
        public string id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public string urlTH { get; set; }
        public string urlEN { get; set; }
        public string start { get; set; }

        public string end { get; set; }

        public bool isLatestUpdate { get; set; }

    }

    public Data data { get; set; }
}
