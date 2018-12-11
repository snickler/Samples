using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValuesPage : ContentPage
    {
        public class Value
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
     

        protected override async void OnAppearing()
        {
            var dataRequest = await App.Client.GetAsync("http://ipaddress:5001/api/values");
            if(dataRequest.IsSuccessStatusCode)
            {
                var resultString = await dataRequest.Content.ReadAsStringAsync();
                var resultObject = JsonConvert.DeserializeObject<List<Value>>(resultString);
                MyListView.ItemsSource = resultObject;
            }
            base.OnAppearing();
        }

        public ValuesPage()
        {
            InitializeComponent();
            
           
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
