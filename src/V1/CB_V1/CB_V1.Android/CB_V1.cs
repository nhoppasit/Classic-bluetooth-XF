using Android.Content;
using System;
using Android.Bluetooth;
using Android.Util;
using Java.Util;
using Newtonsoft;
using Xamarin.Forms;
using CB_V1;

[assembly: Dependency(typeof(ClassicBluetooth))]
namespace CB_V1
{
    public class ClassicBluetooth : IClassicBluetooth
    {
        public string Check()
        {
            throw new NotImplementedException();
        }
    }
}