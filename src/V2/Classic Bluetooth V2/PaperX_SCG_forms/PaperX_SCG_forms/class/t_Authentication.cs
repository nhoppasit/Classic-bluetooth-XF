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


public class p_Authentication
{
    public string username { get; set; }
    public string password { get; set; }
    public string timestamp { get; set; }
    public string checkSum { get; set; }
    public string softwareVersion { get; set; }
    public string notificationToken { get; set; }
    public string languege { get; set; }


}


public class t_Authentication
{

    public class Data
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string ticket { get; set; }
        public droppointData droppointData { get; set; }
        public string syncTime { get; set; }

    }


    public class droppointData
    {
        public string partnerID { get; set; }
        public string partner { get; set; }
        public string locationID { get; set; }
        public string locationName { get; set; }

        public string machineID { get; set; }
        public string machineCode { get; set; }

        public string activeTime { get; set; }
    }

    public Data data { get; set; }



}
