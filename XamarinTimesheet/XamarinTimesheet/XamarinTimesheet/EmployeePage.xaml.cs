using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Diagnostics;
using XamarinTimesheet.Models;
using Xamarin.Forms.Markup;

namespace XamarinTimesheet
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class EmployeePage : ContentPage
    {
        public EmployeePage()
        {
            InitializeComponent();
            listEmployee.ItemsSource = new object[] { "Huoltovarma Oy", "Työn iloa!" };
        }

        private async void btnGetEmployees_Clicked(object sender, EventArgs e)
        {
            listEmployee.ItemsSource = new object[] { "Odota, tietoja ladataan...." };

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://timesheetrestapiske.azurewebsites.net/");
            string json = await client.GetStringAsync("/api/employees/");
            object[] employees = JsonConvert.DeserializeObject<object[]>(json);

            listEmployee.ItemsSource = employees;

        }



        //--- EVENT: LADATAAN TYÖTEHTÄVÄT-SIVU ---//
        private async void btnGetWorkAssignments_Clicked(object sender, EventArgs e)
        {
            string employee = listEmployee.SelectedItem?.ToString();  // ?-operaattori mahdollistaa toString funktion, vaikka arvo puuttuisi

            if (string.IsNullOrEmpty(employee)) //jos työntekijää ei ole valittu
            {
                await DisplayAlert("Henkilövalinta puuttuu", "Valitse listalta työntekijä", "OK"); //(Otsikko, viestin teksti, kuittausnapin teksti)
            }
            else
            {
                //Otetaan työntekijän nimi talteen ja navigoidaan Työtehtävät sivulle
                SelectedEmployee.Name = employee;
                await Navigation.PushAsync(new WorkAssignmentPage());

                //Ks. App.xaml.cs => määrittele EmployeePage:lle navigointi
            }
        }
        //--- BUTTON: siirrytään Geolokaatiosivulle ---//
        private async void NaviGeolocPagelle_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GeolocationPage());
        }
    }
}
