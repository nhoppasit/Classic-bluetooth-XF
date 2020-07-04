using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PaperX_SCG_forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class testXY : ContentPage
    {
        public testXY()
        {
            InitializeComponent();
        }


        private void Btn1_Clicked(object sender, EventArgs e)
        {
            txt_test.Text = "";
        }

        private void Btn2_Clicked(object sender, EventArgs e)
        {
            txt_test.Text = "Hello";
        }

        private void Btn3_Clicked(object sender, EventArgs e)
        {

        }

        private void Btn4_Clicked(object sender, EventArgs e)
        {

        }

        private void Btn5_Clicked(object sender, EventArgs e)
        {

        }

        private void Btn6_Clicked(object sender, EventArgs e)
        {

        }

        private void Btn7_Clicked(object sender, EventArgs e)
        {

        }

        private void Btn8_Clicked(object sender, EventArgs e)
        {

        }

        private void Btn9_Clicked(object sender, EventArgs e)
        {

        }


        private void Btn10_Clicked(object sender, EventArgs e)
        {

        }


    }
}