using Android.Runtime;
using OBDApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OBDApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppView : ContentPage
	{
		public AppView ()
		{
			InitializeComponent ();
            MessagingCenter.Subscribe<ViewModels.BluetoothConnectionHandler>(this, Constants.MESSAGE_DEVICE_SELECTED, (s) => {
                DisplayAlert("Alert","No Device Slected","Cancel");
            });
            MessagingCenter.Subscribe<ViewModels.BluetoothConnectionHandler>(this,"dev", (s) => {
                deviceNameLabel.Text = OBDApp.ViewModels.BluetoothConnectionHandler.SelectedBluetoothDevice;
            });
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeviceListPage());
        }
    }
}