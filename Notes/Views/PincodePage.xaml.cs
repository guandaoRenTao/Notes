using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.ViewModels;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(Flag), nameof(Flag))]
    public partial class PincodePage : ContentPage
    {
        public int Flag
        {
            set
            {
                flag = value;
            }
        }
        int flag;
        public PincodePage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (flag != 0)
            {
                backBtn.IsEnabled = true;
                backBtn.IsVisible = true;
                fingerPrintBtn.IsEnabled = false;
            }
        }
        protected override bool OnBackButtonPressed()
        {
            if(flag == 0)
                return false;
            base.OnBackButtonPressed();
            return true;
        }

        private async void pin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(pin.Text !=null)
            if (pin.Text.Length == 5)
            {
                    if (flag == 1)
                    {
                        Preferences.Set("isLocked", true);
                        Preferences.Set("pincode", pin.Text);
                        await Shell.Current.GoToAsync("..");

                    }
                    else
                    {
                        if (pin.Text == Preferences.Get("pincode", ""))
                        {
                            if (flag == 2)
                                Preferences.Set("isLocked", false);
                            await Shell.Current.GoToAsync("..");

                        }
                        else
                        {
                            await DisplayAlert("Ошибка", "Неправильный ПИН-код. Попробуйте ещё", "Ок");
                            clearBtn.Command.Execute(null);
                        }
                    }
            }
        }


        private async void backBtn_Pressed(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void fingerPrintBtn_Clicked(object sender, EventArgs e)
        {
            var availability = await CrossFingerprint.Current.IsAvailableAsync();
            if (!availability)
            {
                await DisplayAlert("Ошибка", "Нет биометрических данных", "Ок");
                return;
            }
            var result = await CrossFingerprint.Current.AuthenticateAsync(
                new AuthenticationRequestConfiguration("Аутентификация", "Используйте свои биометрические данные"));
            if (result.Authenticated)
            {
                await Shell.Current.GoToAsync("..");
            }
        }


        //private async void pin_Completed(object sender, EventArgs e)
        //{
        //    if (Preferences.Get("pincode", "") == pin.Text)
        //        await Navigation.PopModalAsync();
        //    else
        //        wrongPin.IsVisible = true;
        //}
    }
}