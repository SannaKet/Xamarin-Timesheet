using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinTimesheet
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Tämä navigointi osuus ei ole vakiona projektissa, vaan täytyy määrittää itse. Nyt navigointi toimii.

            MainPage = new NavigationPage (new EmployeePage()); 
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
