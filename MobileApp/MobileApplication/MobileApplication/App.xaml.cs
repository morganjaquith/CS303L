using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MobileApplication.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MobileApplication
{
    public partial class App : Application
    {
        public static string Username { get; set; }
        public static string Password { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new FrontPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
