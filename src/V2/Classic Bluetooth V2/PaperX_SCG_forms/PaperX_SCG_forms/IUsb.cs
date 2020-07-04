using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaperX_SCG_forms
{
    public interface IUsb
    {
        int LastResult { get; }
        string LastResultMessage { get; }
        Task Stop();
        Task<string> HomeShutter(int deviceIndex, long waitTime, Label label);
        Task<string> OpenShutter(int deviceIndex, long waitTime, Label label);
        Task<string> CloseShutter(int deviceIndex, long waitTime, Label label);
        Task<string> Kick(int deviceIndex, long waitTime, Label label);
        Task<string> HomeKicker(int deviceIndex, long waitTime, Label label);
        Task<string> Weighting(int deviceIndex, long waitTime);
        Task<string> ReadSensor(int iSen, int deviceIndex, long waitTime);
        Task<string> ReadAllSensor(int deviceIndex, long waitTime);
        Task<int[]> FindDevices();
    }
}
