using Xamarin.Forms;
using Notes.Views;
using Notes.Models;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using Rg.Plugins.Popup.Services;

namespace Notes
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Tab tab { get; set; }
        public Command Command { get; private set; }
        public Command AddCategoryCommand { get; private set; }
        public Command<Category> EditCategoryCommand { get; private set; }
        public Command ToggleThemeCommand { get; private set; }
        public Command<Category> EditCommand { get; private set; }
        public Command<Category> DeleteCommand { get; private set; }

        bool isDark = Preferences.Get("currentTheme", "DarkTheme") == Themes.ThemesList.Themes.DarkTheme.ToString();
        public bool IsDarkTheme
        {
            get { return isDark; }
            set { isDark = value; NotifyPropertyChanged(nameof(IsDarkTheme)); }
        }
        public CollectionView folders { get; private set; }
        public AppShell()
        {
            InitializeComponent();
            folders = foldersList;
            /*Command = new Command(
                execute: () =>
                {
                    tab.Items[0] = new ShellContent { Content = new CategoryListPage()};
                    FlyoutIsPresented = false;
                });*/
            AddCategoryCommand = new Command(
                execute: async() =>
                {
                    FlyoutIsPresented = false;
                    await PopupNavigation.Instance.PushAsync(new CategoryPopup());
                });
            EditCategoryCommand = new Command<Category>(
                execute: async(Category folder) =>
                {
                    if (folder.ID != -1)
                    {
                        FlyoutIsPresented = false;
                        await PopupNavigation.Instance.PushAsync(new CategoryPopup(folder));
                    }
                });
            ToggleThemeCommand = new Command(
                execute: () =>
                {
                    toggleThemeAnimation();
                    if (!IsDarkTheme)
                        SettingsPage.ChangeTheme(Themes.ThemesList.Themes.DarkTheme.ToString());
                    else
                        SettingsPage.ChangeTheme(Themes.ThemesList.Themes.LightTheme.ToString());
                    
                });
            /*EditCommand = new Command<Category>(
                execute: (Category folder) =>
                {
                    tab.Items[0] = new ShellContent { Content = new CategoryListPage()};
                    FlyoutIsPresented = false;
                });
            DeleteCommand = new Command<Category>(
                execute: (Category folder) =>
                {
                    App.Database.DeleteCategoryWithNotesAsync(folder);
                    init();
                });*/
            Routing.RegisterRoute(nameof(NoteEntryPage), typeof(NoteEntryPage));
            Routing.RegisterRoute(nameof(NotesListPage), typeof(NotesListPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(PincodePage), typeof(PincodePage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
            BindingContext = this;
            tab = myTab;
            init();

        }

        public async void toggleThemeAnimation()
        {
            await Task.WhenAll(
            theme.FadeTo(0, 150),
            theme.ScaleTo(0,150));
            IsDarkTheme = !IsDarkTheme;
            await Task.WhenAll(
            theme.FadeTo(1, 150),
            theme.ScaleTo(1, 150));
        }

        public async void init(bool f = false)
        {
            var folders1 = await App.Database.GetCategoriesAsync();
            List<Category> folders = new List<Category>();
            folders.Add(new Category() { Name = "Все заметки", ID = -1 });
            
            folders.AddRange(folders1);
            foldersList.ItemsSource = folders;
            if (f)
                foldersList.SelectedItem = folders.FirstOrDefault();
            
        }

        private void foldersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count != 0)
            {
                Category folder = (Category)e.CurrentSelection.FirstOrDefault();
                tab.Items[0] = new ShellContent { Content = new NotesListPage() { CategoryId = folder.ID, Title = folder.Name, IconImage = folder.Icon} };
                FlyoutIsPresented = false;
            }

        }

    }
}