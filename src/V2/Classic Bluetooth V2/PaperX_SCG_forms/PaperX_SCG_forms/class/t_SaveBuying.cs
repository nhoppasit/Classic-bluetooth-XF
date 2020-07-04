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


class p_SaveBuying
{
    public string customerQR { get; set; }
    public int productId { get; set; }
    public double weight { get; set; }
    public string transactionDate { get; set; }
}

class t_SaveBuying
{
    public class Data
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public Data data { get; set; }
}
