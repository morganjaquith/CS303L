using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileApplication.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
		}

        void SignIn(object sender, EventArgs e)
        {
            Database db = new Database();
            if (db.UserLogin(username.Text, password.Text))
            {
                Application.Current.MainPage = new MainPage();
            }
        }
	}
}