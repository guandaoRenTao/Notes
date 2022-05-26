using System;
using System.IO;
using Notes.Data;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.Generic;
using Notes.Models;
using Notes.Views;
using Notes.ViewModels;
namespace Notes
{
    public partial class App : Application
    {
        static NoteDatabase database;

        // Create the database connection as a singleton.
        public static NoteDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new NoteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
                }
                return database;
            }
        }

        public App()
        {
            /* Назначает светлую тему
            Current.RequestedThemeChanged += (s, a) =>
            {
                Current.UserAppTheme = OSAppTheme.Light;
            };
            Application.Current.UserAppTheme = OSAppTheme.Light;*/
            InitializeComponent();
            MainPage = new AppShell();
            //todo:
            //Изменить Preferences на Current.Properties
            //Application.Current.Properties["currentTheme"];
            if (Preferences.Get("currentTheme", null) == null)
                Preferences.Set("currentTheme", Themes.ThemesList.Themes.DarkTheme.ToString());
            else
            {
                SettingsPage.ChangeTheme(Preferences.Get("currentTheme", null));
            }
            /*if (Preferences.Get("accentColor", null) == null)
                Preferences.Set("accentColor", "Red");
            else
            {
                SettingsPage.ChangeAccent(Preferences.Get("accentColor", null));
            }*/
            if (Preferences.Get("toggleView", false) == false)
                Preferences.Set("toggleView", false);

            this.BindingContext = new MainViewModel();
        }

        protected async override void OnStart()
        {
            if (((MainViewModel)BindingContext).IsLocked)
            {
                //await MainPage.Navigation.PushModalAsync(new PincodePage(0));
                await Shell.Current.GoToAsync($"{nameof(PincodePage)}?{nameof(PincodePage.Flag)}={0}");
            }
        }

        protected override void OnSleep()
        {
            //MessagingCenter.Send<App, bool>(this, "clearSelection", true);
        }

        protected override void OnResume()
        {
        }
    }
}