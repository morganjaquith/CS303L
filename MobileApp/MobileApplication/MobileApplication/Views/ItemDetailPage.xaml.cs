using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using MobileApplication.Models;
using MobileApplication.ViewModels;

namespace MobileApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();
            ButtonEditItem.Clicked += ButtonEditItem_Clicked;
            ButtonDeleteItem.Clicked += ButtonDeleteItem_Clicked;
            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new Item
            {
                Text = "Item 1",
                Description = "This is an item description."
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }

        private void ButtonEditItem_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonDeleteItem_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}