using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
        }

        bool isLocked = Preferences.Get("isLocked", false);
        string pincode = Preferences.Get("pincode", "");
        public bool IsLocked
        {
            set
            {
                if (isLocked != value)
                {
                    isLocked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLocked"));
                }
            }
            get
            {
                return isLocked;
            }
        }

        public string Pincode
        {
             set
            {
                if (pincode != value)
                {
                    pincode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Pincode"));
                }
            }
            get
            {
                return pincode;
            }
        }
    }
}
