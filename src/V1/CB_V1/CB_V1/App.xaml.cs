using CB_V1.Classes;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CB_V1
{
    public partial class App : Application
    {
        public static HardwareClass Hardware;

        public App()
        {
            InitializeComponent();

            Hardware = new HardwareClass();

            //MainPage = new MainPage();
            MainPage = new Views.Page1();
            
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
