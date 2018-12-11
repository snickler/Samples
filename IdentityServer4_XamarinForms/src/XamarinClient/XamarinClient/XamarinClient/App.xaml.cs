using System;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using Xamarin.Auth.Presenters;
using System.Net.Http;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XamarinClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
       
        public static Account AuthAccount { get; set; }
        public static HttpClient Client = new HttpClient();
        protected override void OnStart()
        {
            var oAuth = new OAuth2AuthenticatorEx("xamarin-client", "offline_access values-api",
                new Uri("http://ipaddress:5000/connect/authorize"), new Uri("http://ipaddress:5000/grants"))
            {
                AccessTokenUrl = new Uri("http://ipaddress:5000/connect/token"),
                ShouldEncounterOnPageLoading = false
            };
            var account = AccountStore.Create().FindAccountsForService("AuthServer");
            if (account != null && account.Any())
            {
                AuthAccount = account.First();
                Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AuthAccount.Properties["access_token"]}");
                MainPage = new ValuesPage();
            }
            else
            {
                var presenter = new OAuthLoginPresenter();
                presenter.Completed += Presenter_Completed;
                presenter.Login(oAuth);
            }
        }

        private void Presenter_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            if(e.IsAuthenticated)
            {

                AuthAccount = e.Account;
                Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AuthAccount.Properties["access_token"]}");
            //    await AccountStore.Create().SaveAsync(e.Account, "AuthServer");
                MainPage = new ValuesPage();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
