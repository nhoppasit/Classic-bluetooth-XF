
using FormsVideoLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaperX_SCG_forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class test : ContentPage
    {
        public test()
        {
            InitializeComponent();

            //string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Android.OS.Environment.DataDirectory.AbsolutePath);
            //string file = Path.Combine(directory, "abs_th.mp4");


            //   var file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("/files","/"), "Advertise/abs_th.mp4");


            var currentDir = Directory.GetCurrentDirectory();
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Advertise/abs_th.mp4");

            //  var file =  Path.Combine(a.List("").ToString(), "Advertise/abs_th.mp4");

            if (!String.IsNullOrWhiteSpace(file))
            {
                videoPlayer.Source = new FileVideoSource
                {
                    File = file
                };
            }

        }
    }
}