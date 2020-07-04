using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using PaperX_SCG_forms.Droid;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Java.Lang;
using Android.Util;

[assembly: Dependency(typeof(AndroidReboot))]
namespace PaperX_SCG_forms.Droid
{
   public class AndroidReboot : IRebootMachine
    {
        [Obsolete]
        public void RebootMachine()
        {
            //try
            //{
            //    Permission permissionCheck = ContextCompat.CheckSelfPermission(Forms.Context, Manifest.Permission.Reboot);

            //    if (permissionCheck != Permission.Granted)
            //    {
            //        // Android.Support.V4.App.ActivityCompat.RequestPermissions(Xamarin.Forms.Platform.Android.FormsApplicationActivity, new string[] { Manifest.Permission.Reboot },  0);
            //        Android.Support.V4.App.ActivityCompat.RequestPermissions(globalVariables.activity,
            //   new string[] { Manifest.Permission.Reboot },
            //   1);
            //    }
            //    else
            //    {
            //        //TODO
            //    }
            //    try
            //    {
            //        Runtime.GetRuntime().Exec(new string[] { "su", "-c", "am force-stop com.android.launcher" });
            //    }
            //    catch (System.Exception e)
            //    {
            //        e = e;
            //        //do something
            //    }
            //    PowerManager pm = (PowerManager)Forms.Context.GetSystemService(Context.PowerService);

            //    pm.Reboot(null);
            //}
            //catch (System.Exception ex)
            //{
            //    ex = ex;
            //}
            //try
            //{
            //    Runtime.GetRuntime().Exec(new string[] { "/system/bin/su", "-c", "reboot now" });
            //}
            //catch (System.Exception ex)
            //{
            //    ex = ex;
            //}
            //try
            //{
            //    Runtime.GetRuntime().Exec(new string[] { "su", "-c", "reboot now" });
            //}
            //catch (System.Exception ex)
            //{
            //    ex = ex;
            //}

            //  Android.OS.Process.KillProcess(Android.OS.Process.MyPid());

            string command = "adb reboot";
            try
            {
                Java.Lang.Process sh = Runtime.GetRuntime().Exec("su", null, null);
                System.IO.Stream os = sh.OutputStream;
                byte[] mScreenBuffer = Encoding.Unicode.GetBytes(command);
                os.Write(mScreenBuffer);
                os.Flush();
                os.Close();
                sh.WaitFor();
                Log.Verbose("PPX", "comple");
            }
            catch(Java.Lang.Exception e)
            {
                e.PrintStackTrace();
            }
    }
    }


}

