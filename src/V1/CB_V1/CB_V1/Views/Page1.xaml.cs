using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CB_V1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();

            btnHello.Clicked += BtnHello_Clicked;
        }

        private void BtnHello_Clicked(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}