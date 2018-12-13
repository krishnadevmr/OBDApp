using OBDApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OBDApp.ViewModels
{
    public class PairedDeviceViewModel : BaseViewModel
    {
        public ObservableCollection<PairedDevice> pairedDevces { get; set; }

        public PairedDeviceViewModel()
        {
            this.pairedDevces = new ObservableCollection<PairedDevice>();
            this.pairedDevces.Add(new PairedDevice { DeviceName = "Device 1" });
        }
    }
}
