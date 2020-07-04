using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using PaperX_SCG_forms;
using System.Threading.Tasks;
using Android.Util;
using Newtonsoft.Json;
using PaperX_SCG_forms.Views;

namespace PaperX_SCG_forms.Classes
{
    public class HardwareDataClass
    {
        public int McuVenderId = 0;
        public int McuProductId = 0;

        public int InidicatorVenderId = 0;
        public int IndicatorProductId = 0;

        public int McuDeviceIndex = -1;
        public int IndicatorDeviceIndex = -1;

        public bool Ready;
    }
}
