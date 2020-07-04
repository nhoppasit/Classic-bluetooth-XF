using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CB_V1.Classes
{
    public class HardwareClass
    {
        public IClassicBluetooth Bluetooth;

        public HardwareClass()
        {
            Bluetooth = DependencyService.Get<IClassicBluetooth>();
        }

    }
}
