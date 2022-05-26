using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Themes;
namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public Command<string> ChangeColorCommand { get; private set; }

        public SettingsPage()
        {
            InitializeComponent();
            /*if (Preferences.Get("accentColor", null) == null)
                Preferences.Set("accentColor", Color.Red.ToString());*/

            ChangeColorCommand = new Command<string>(
                execute: (string color) =>
                {
                    ChangeAccent(color);
                });
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(Preferences.Get("currentTheme", null) == "DarkTheme")
                switchBtn2.IsToggled = true;
            switchBtn1.IsToggled = Preferences.Get("isLocked", false);
            //MyListView.ItemsSource = Enum.GetNames(typeof(ThemesList.Themes));
        }
        public static void ChangeTheme(string theme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                Preferences.Set("currentTheme", theme);
                
                switch (theme)
                {
                    case "LightTheme":
                        {
                            mergedDictionaries.Add(new LightTheme());
                            break;
                        }
                    case "DarkTheme":
                        {
                            mergedDictionaries.Add(new DarkTheme());
                            break;
                        }
                    default:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                }
                ChangeAccent(Preferences.Get("accentColor", "Blue"));
            }
        }

        public static void ChangeAccent(string accent)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            switch (accent)
            {
                case "Red":
                    {
                        mergedDictionaries.Add(new RedAccent());
                        break;
                    }
                case "Purple":
                    {
                        mergedDictionaries.Add(new PurpleAccent());
                        break;
                    }
                case "Green":
                    {
                        mergedDictionaries.Add(new GreenAccent());
                        break;
                    }
                case "Blue":
                    {
                        mergedDictionaries.Add(new BlueAccent());
                        break;
                    }
                case "Salmon":
                    {
                        mergedDictionaries.Add(new SalmonAccent());
                        break;
                    }
                default:
                    mergedDictionaries.Add(new PurpleAccent());
                    break;
            }
            Preferences.Set("accentColor", accent);
        }

        /*private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                string theme = (string)e.CurrentSelection.FirstOrDefault();
                ChangeTheme(theme);
            }
        }*/

        private async void switchBtn_Toggled(object sender, ToggledEventArgs e)
        {

            switchBtn1.IsEnabled = false;
            if (e.Value && !Preferences.Get("isLocked", false))
            {
                await Shell.Current.GoToAsync($"{nameof(PincodePage)}?{nameof(PincodePage.Flag)}={1}");
            }
            else
            {
                if (!e.Value && Preferences.Get("isLocked", false))
                    await Shell.Current.GoToAsync($"{nameof(PincodePage)}?{nameof(PincodePage.Flag)}={2}");

            }
            switchBtn1.IsEnabled = true;
        }

        private void switchBtn2_Toggled(object sender, ToggledEventArgs e)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            mergedDictionaries.Clear();
            if (e.Value)
            {
                mergedDictionaries.Add(new DarkTheme());
                Preferences.Set("currentTheme", "DarkTheme");
                ((AppShell)Shell.Current).IsDarkTheme = true;
            }
            else
            {
                mergedDictionaries.Add(new LightTheme());
                Preferences.Set("currentTheme", "LightTheme");
                ((AppShell)Shell.Current).IsDarkTheme = false;
            }
            ChangeAccent(Preferences.Get("accentColor", "Blue"));

        }

        //private async void PasswordSave(object sender, EventArgs e)
        //{
        //    Preferences.Set("isLocked", true);
        //    if (Preferences.Get("password", null) == null)
        //    {
        //        if(newPass.Text != oldPass.Text)
        //        {
        //            await DisplayAlert("Ошибка", "Пароли не совпадают. Попробуйте ещё", "Ок");
        //        }
        //        else
        //        {
        //            Preferences.Set("password", newPass.Text);
        //            await DisplayAlert("Отлично!", "Пароль сохранён", "Ок");
        //        }
        //    }
        //    else
        //    {
        //        if(oldPass.Text != Preferences.Get("password", null))
        //        {
        //            await DisplayAlert("Ошибка", "Неверный старый пароль. Попробуйте ещё", "Ок");
        //        }
        //        else
        //        {
        //            Preferences.Set("password", newPass.Text);
        //            await DisplayAlert("Отлично!", "Пароль сохранён", "Ок");
        //        }
        //    }
        //}
    }
}