using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinTimesheet.Models
{
    //luokka jonka avulla hallitaan valitun Xamarin sovelluksesta valittua työntekijää
    //staattinen luokka, jolle ei perusteta instanssia
    //tätä voidaan lukea mistä tahansa sivusta ja asettaa arvo => SelectedEmployee.Name
    //MIKSI STATIC?

    public static class SelectedEmployee
    {
        public static string Name { get; set; }
        public static string StartComment { get; set; }
        public static string FinalComment { get; set; }
    }
}
