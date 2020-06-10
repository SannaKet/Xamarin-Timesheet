using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinTimesheet.Models;

namespace XamarinTimesheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkAssignmentPage : ContentPage
    {
        public WorkAssignmentPage()
        {
            InitializeComponent();
            listWorkAssignments.ItemsSource = new string[] { "" };
        }


        // ---------------------- EVENT - LADATAAN KAIKKI TEHTÄVÄT TIETOKANNASTA FRONTIIN ----------------------------------------//
        private async void btnGetWorkAssignments_Clicked(object sender, EventArgs e)
        {
            //viesti, ennen kuin näytetään palvelimelta tuleva sisältö
            listWorkAssignments.ItemsSource = new string[] { "Odota...ladataan tietoja" };

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://timesheetrestapiske.azurewebsites.net/");
                string json = await client.GetStringAsync("/api/workassignments");
                string[] assignments = JsonConvert.DeserializeObject<string[]>(json);

                listWorkAssignments.ItemsSource = assignments;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                listWorkAssignments.ItemsSource = new string[] { errorMessage };
            }

            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }

                if (location == null)
                {
                    //lblGeo.Text = "Ei GPS-tietoja";
                    lblLatitude.Text = "Ei GPS-tietoja";
                }
                else
                {
                    lblLatitude.Text = ($"Lat: {location.Latitude}");
                    lblLongitude.Text = ($"Lon: {location.Longitude}");
                }
            }

            catch (Exception ex)
            {
                lblLatitude.Text = ($"Jokin meni vikaan: {ex.Message}");
            }
        }

        // ----------------------- EVENT - ALOITA TYÖ ------------------------------------------//
            private async void btnStartWork_Clicked(object sender, EventArgs e)
        {

            string latitude = lblLatitude.Text;
            string longitude = lblLongitude.Text;

            //Tarkistetaan ensin, onko näkymässä valittu työtä. Muutetaan se string-muotoon
            string assignmentName = listWorkAssignments.SelectedItem?.ToString();


            if (string.IsNullOrEmpty(assignmentName))
            {
                await DisplayAlert("Työ puuttuu", "Valitse ensin aloitettava työ", "OK");
            }

            else
            {

                //---- Jostain syystä LOPETA TYÖN kommentti ei tallentunut tietokantaan, jos tämä ALOITA TYÖ kommentti oli siellä jo. ----//

                string comment = await DisplayPromptAsync("Työn kommentti", "Kirjoita kommentti");
                if (string.IsNullOrEmpty(comment))
                {
                    SelectedEmployee.StartComment = "";
                }
                else
                {
                    SelectedEmployee.StartComment = comment;
                }



                try
                {

                    //Käytetään perustettua WorkAssignmentOperationModel-luokkaa ja luodaan objekti palvelimelle lähetettäväksi
                    WorkAssignmentOperationModel operationData = new WorkAssignmentOperationModel()
                    {
                        Operation = "Start",
                        AssignmentTitle = assignmentName,
                        Name = SelectedEmployee.Name,
                        Comment = SelectedEmployee.StartComment,
                        Latitude = latitude,
                        Longitude = longitude
                    };



                    //HTTP-yhteyden alustaminen
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://timesheetrestapiske.azurewebsites.net/");


                    //Muutetaan operationData => JSON:ksi
                    string operationInput = JsonConvert.SerializeObject(operationData);
                    StringContent operationContent = new StringContent(operationInput, Encoding.UTF8, "application/json");


                    //Lähetetään serialisoitu objekti  back-endille POST-pyyntönä
                    HttpResponseMessage operationMessage = await client.PostAsync("api/workassignments", operationContent);


                    //Otetaan vastaan palvelimen vastaus
                    string operationReply = await operationMessage.Content.ReadAsStringAsync();

                    //Asetetaan operationReply deserialisoituna operationSuccess -muuttujaan
                    bool operationSuccess = JsonConvert.DeserializeObject<bool>(operationReply);


                    //Näytetään mobiilissa kuittausviesti
                    if (operationSuccess)
                    {
                        await DisplayAlert("Työn aloitus", "Työ on aloitettu", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Työn aloitus", "Työtä ei voi aloittaa, koska se on jo aloitettu.", "Sulje");
                    }

                }
                catch (Exception ex) //otetaan poikkeus kiinni muuttujaan ex ja ja sijoitetaan errorMessageen
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;  //poikkeuksen selvittäminen 
                    listWorkAssignments.ItemsSource = new string[] { errorMessage };  //ja näytetään listViewssä mobiilissa
                }
            }
        }



        // --------------------------- EVENT - LOPETA TYÖ --------------------------------------//
        private async void btnStopWork_Clicked(object sender, EventArgs e)
        {

            //Tarkistetaan ensin, onko käyttäjä valinnut työn
            string assignmentName = listWorkAssignments.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(assignmentName))
            {
                await DisplayAlert("Työ puuttuu", "Valitse ensin aloitettava työ", "OK");
            }

            //Jos valinta on tehty, niin......
            else
            {

                string comment = await DisplayPromptAsync("Työn kommentti", "Kirjoita kommentti");
                if (string.IsNullOrEmpty(comment))
                {
                    SelectedEmployee.FinalComment = "";
                }
                else
                {
                    SelectedEmployee.FinalComment = comment;
                }
                try

                {
                    //Käytetään perustettua WorkAssignmentOperationModel-luokkaa ja luodaan objekti palvelimelle lähetettäväksi
                    WorkAssignmentOperationModel operationData = new WorkAssignmentOperationModel()
                    {
                        Operation = "Stop",
                        AssignmentTitle = assignmentName,
                        Name = SelectedEmployee.Name,
                        Comment = SelectedEmployee.FinalComment
                    };



                    //HTTP-yhteyden perusta
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://timesheetrestapiske.azurewebsites.net/");


                    //Muutetaan operationData => JSON:ksi
                    string operationInput = JsonConvert.SerializeObject(operationData);
                    StringContent operationContent = new StringContent(operationInput, Encoding.UTF8, "application/json");


                    //Lähetetään serialisoitu objekti  back-endille POST-pyyntönä
                    HttpResponseMessage operationMessage = await client.PostAsync("api/workassignments", operationContent);


                    //Otetaan vastaan palvelimen vastaus
                    string operationReply = await operationMessage.Content.ReadAsStringAsync();

                    //Asetetaan operationReply deserialisoituna operationSuccess -muuttujaan
                    bool operationSuccess = JsonConvert.DeserializeObject<bool>(operationReply);



                    //Näytetään mobiilissa kuittausviesti
                    if (operationSuccess)
                    {
                        await DisplayAlert("Työn lopetus", "Työ on lopetettu", "OK");
                        listWorkAssignments.ItemsSource = "";
                    }
                    else
                    {
                        await DisplayAlert("Työn lopetus", "Työtä ei voitu lopettaa, koska sitä ei ole aloitettu", "Sulje");
                    }

                }
                catch (Exception ex) //otetaan poikkeus kiinni muuttujaan ex ja ja sijoitetaan errorMessageen
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;  //poikkeuksen selvittäminen 
                    listWorkAssignments.ItemsSource = new string[] { errorMessage };  //ja näytetään listViewssä mobiilissa
                }
            }
        }
    }
}