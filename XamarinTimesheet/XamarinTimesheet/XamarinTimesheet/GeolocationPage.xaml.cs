using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinTimesheet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeolocationPage : ContentPage
    {
        public GeolocationPage()
        {
            InitializeComponent();
        }

        //--- EVENT: Haetaan geolokaation annetun osoitteen perusteella
        private async void btnGetLocation_Clicked(object sender, EventArgs e)
        {
            try
            {
                //var address = "Microsoft Building 25 Redmond WA USA";
                var address = AddressInput.Text;
                var locations = await Geocoding.GetLocationsAsync(address);

                var location = locations?.FirstOrDefault();
                if (location != null)
                {
                    lblGeoLat.Text = ($"Lat: {location.Latitude}");
                    lblGeoLon.Text = ($"Lon: {location.Longitude}");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                lblGeoLat.Text = "Jokin meni vikaan: " + ex.Message;
            }

        }

        //--- EVENT: Haetaan osoite annetun geolokaation perusteella
        private async void btnGetAddress_Clicked(object sender, EventArgs e)
        {
            try
            {
                //var lat = 47.673988;
                //var lon = -122.121513;

                var lat = float.Parse(GeoLatitude.Text);
                var lon = float.Parse(GeoLongitude.Text);


                var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var geocodeAddress =
                        $"AdminArea:       {placemark.AdminArea}\n" +
                        $"CountryCode:     {placemark.CountryCode}\n" +
                        $"CountryName:     {placemark.CountryName}\n" +
                        $"FeatureName:     {placemark.FeatureName}\n" +
                        $"Locality:        {placemark.Locality}\n" +
                        $"PostalCode:      {placemark.PostalCode}\n" +
                        $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                        $"SubLocality:     {placemark.SubLocality}\n" +
                        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                        $"Thoroughfare:    {placemark.Thoroughfare}\n"
                        ;

                    lblGeoAddress.Text = geocodeAddress;
                }

                else
                {
                    lblGeoAddress.Text = "Tietoja ei valitettavasti ole saatavissa";
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                lblGeoAddress.Text = "Jokin meni vikaan: " + ex.Message;
            }
        }




        private async void btnDistanceButton_Clicked(object sender, EventArgs e)
        {

            try
            {
                //Nykyinen sijainti
                var currentLocation = await Geolocation.GetLastKnownLocationAsync();

                if (currentLocation == null)
                {
                    currentLocation = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }


                //Kohdesijainti
                var address = Destination.Text;
                var locations = await Geocoding.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();


                double distanceBetween = Math.Round(Location.CalculateDistance(currentLocation, location.Latitude, location.Longitude, DistanceUnits.Kilometers),1);



                if (currentLocation == null)
                {
                    lblDistance.Text = "Ei GPS-tietoja nykyisestä sijainnistasi";
                }
                else
                {
                    if (location != null) {
                        lblDistance.Text = ($"Etäisyys pyytämääsi kohteeseen on: {distanceBetween} km");
                    }
                    else
                    {
                        lblDistance.Text = ("Matkaa ei voida laskea!");
                    }
                }
            }

            catch (Exception ex)
            { 
                lblDistance.Text = ($"Jokin meni vikaan: {ex.Message}");
            }

            //catch (FeatureNotSupportedException fnsEx)
            //{
            //    // Handle not supported on device exception
            //}
            //catch (FeatureNotEnabledException fneEx)
            //{
            //    // Handle not enabled on device exception
            //}
            //catch (PermissionException pEx)
            //{
            //    // Handle permission exception
            //}


        }



        private async void btnBackToEmpoloyeePage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }


    }
}