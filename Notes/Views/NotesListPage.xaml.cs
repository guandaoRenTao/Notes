using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Notes.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]
    public partial class NotesListPage : ContentPage
    {
        
        
        private string noteShape;
        public string NoteShape
        {
            get { return noteShape; }
            set
            {
                noteShape = value;
                OnPropertyChanged();
            }
        }
        public int CategoryId {
            get
            {
                return folderId;
            }
            set
            {
                folderId = value;
            }
        }
        int folderId = -1;
        public bool Toggle { get; set; }
        string iconImage = "";
        public string IconImage
        {
            get { return iconImage; }
            set
            {
                iconImage = value;
                OnPropertyChanged();
            }
        }

        public bool longPress = true;
        public bool LongPressEnabled
        {
            get { return !longPress; }
            set { longPress = value; OnPropertyChanged(); }
        }
        public Command<Note> LongPressCommand { get; private set; }
        public Command<Note> PressCommand { get; private set; }
        public Command AddPressCommand { get; private set; }
        public Command SettingsCommand { get; private set; }
        public Command ToggleViewCommand { get; private set; }

        public NotesListPage()
        {
            InitializeComponent(); 
            /*MessagingCenter.Subscribe<App, bool>(this, "clearSelection", (sender, arg) =>
            {
                TempF(false);
            });*/
            LongPressCommand = new Command<Note>(
                execute: (Note note) =>
                {
                    LongPress(note);
                });
            /*PressCommand = new Command<Note>(
                execute: async (Note note) =>
                {
                    IsBusy = true;
                    await Shell.Current.GoToAsync($"{nameof(NoteEntryPage)}?{nameof(NoteEntryPage.ItemId)}={note.ID}");
                    IsBusy = false;
                });*/
            AddPressCommand = new Command(
                execute: async() =>
                {
                    await Shell.Current.GoToAsync($"{nameof(NoteEntryPage)}?{nameof(NoteEntryPage.CategoryId)}={folderId}");
                });
            SettingsCommand = new Command(
                execute: () =>
                {
                    popup.IsOpen = false;
                    SettingsClicked(this, EventArgs.Empty);
                });
            ToggleViewCommand = new Command(
                execute: () =>
                {
                    popup.IsOpen = false;
                    toggleView(this, EventArgs.Empty);
                });
            BindingContext = this;
            Toggle = Preferences.Get("toggleView", false);
            Toggle = !Toggle;
            toggleView(this, EventArgs.Empty);
        }
        async void init()
        {
            IsBusy = true;
            MyListView.SelectedItems.Clear();
            List<Note> notes = new List<Note>();
            if (folderId == -1)
                notes = await App.Database.GetAllNotesAsync();
            else
                notes = await App.Database.GetNotesAsync(folderId);
            MyListView.ItemsSource = notes.Reverse<Note>();
            IsBusy = false;

        }
        protected override void OnAppearing()
        {
            init();
            base.OnAppearing();
            FadeEffect(MyListView, 1);
            //var notes = await App.Database.GetNotesAsync(CategoryId);
        }
        
        protected override void OnDisappearing()
        {
            FadeEffect(MyListView, 0);
            base.OnDisappearing();
        }
        public void LongPress(Note note)
        {
            if (longPress)
            {
                Vibration.Vibrate(5);
                SelectClicked(deleteBtn, EventArgs.Empty);
                MyListView.SelectedItems.Add(note);
                LongPressEnabled = false;
            }
        }
        private void menuBtn_Pressed(object sender, EventArgs e)
        {
            /*if (folderId == -1)
                MyListView.ItemsSource = await App.Database.GetAllNotesAsync();
            else
                MyListView.ItemsSource = await App.Database.GetNotesAsync(folderId);*/
            AppShell.Current.FlyoutIsPresented = true;
        }
        
        void FadeEffect(VisualElement element, int opacity)
        {
            element.FadeTo(opacity,300,Easing.Linear);
        }
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsBusy = true;

            if (e.CurrentSelection.Count != 0)
            {
                Note note = (Note)e.CurrentSelection.FirstOrDefault();
                //await Navigation.PushModalAsync(new NoteEntryPage(note));
                await Shell.Current.GoToAsync($"{nameof(NoteEntryPage)}?{nameof(NoteEntryPage.ItemId)}={note.ID}");
                ((CollectionView)sender).SelectedItem = null;
            }
            IsBusy = false;

        }
        void OnMultiSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                /*var sel = e.CurrentSelection;
                
                if (sel.Where(a => a == sel[sel.Count - 1]).Count() > 1)*/
                    countLabel.Text = "(" + MyListView.SelectedItems.Count().ToString() + ") ";
            }
        }

        async void SettingsClicked(object sender, EventArgs e)
        {
            toolBtn.IsEnabled = false;
            await Shell.Current.GoToAsync(nameof(SettingsPage));
            toolBtn.IsEnabled = true;
        }

        private void SelectClicked(object sender, EventArgs e)
        {
            TempF(true);
        }

        private void ClearSelection(object sender, EventArgs e)
        {
            TempF(false);
        }

        private void toggleView(object sender, EventArgs e)
        {
            if (Toggle)
            {
                //toggleViewBtn.Style = (Style)Resources["Style1"];
                MyListView.ItemsLayout = new GridItemsLayout(1, ItemsLayoutOrientation.Vertical) { HorizontalItemSpacing = 10, VerticalItemSpacing = 10 };
                Toggle = false;
                NoteShape = "Плитки";
            }
            else
            {
                //toggleViewBtn.Style = (Style)Resources["Style2"];
                MyListView.ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { HorizontalItemSpacing = 10, VerticalItemSpacing = 10 };
                Toggle = true;
                NoteShape = "Список";
            }
            Preferences.Set("toggleView", Toggle);
        }

        void TempF(bool n)
        {
            if (n)
            {

                MyListView.SelectionChanged -= OnSelectionChanged;
                MyListView.SelectedItem = null;
                FadeEffect(searchBtn, 0);
                FadeEffect(deleteBtn, 1);
                FadeEffect(clearBtn, 1);
                FadeEffect(countLabel, 1);
                FadeEffect(menuBtn, 0);
                menuBtn.IsEnabled = false;
                searchBtn.IsEnabled = false;
                deleteBtn.IsEnabled = true;
                clearBtn.IsEnabled = true;
                MyListView.SelectionMode = SelectionMode.Multiple;
                toolBtn.IsEnabled = false;
                plus.IsEnabled = false;
                MyListView.SelectionChanged += OnMultiSelectionChanged;
            }
            else
            {
                FadeEffect(countLabel, 0);
                FadeEffect(deleteBtn, 0);
                FadeEffect(clearBtn, 0);
                FadeEffect(menuBtn, 1);
                FadeEffect(searchBtn, 1);
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Delay(200);
                    menuBtn.IsEnabled = true;
                    searchBtn.IsEnabled = true;
                });
                deleteBtn.IsEnabled = false;
                clearBtn.IsEnabled = false;
                MyListView.SelectedItems.Clear();
                MyListView.SelectionChanged -= OnMultiSelectionChanged;
                MyListView.SelectionChanged += OnSelectionChanged;
                toolBtn.IsEnabled = true;
                plus.IsEnabled = true;
                MyListView.SelectionMode = SelectionMode.Single;
                LongPressEnabled = true;
            }
        }
        private async void deleteBtn_Pressed(object sender, EventArgs e)
        {
            if (MyListView.SelectedItems.Count != 0)
            {
                bool answer = await DisplayAlert("Удаление (" + MyListView.SelectedItems.Count + ")", "Удалить? ", "Да", "Отмена");
                if (answer)
                {
                    foreach (Note note in MyListView.SelectedItems)
                    {
                        if (note != null)
                            await App.Database.DeleteNoteAsync(note);
                    }
                    init();

                }
                ClearSelection(deleteBtn, e);
            }
        }

        /*private async void searchBtn_SearchButtonPressed(object sender, EventArgs e)
        {
            string text = searchBtn.Text.ToLower();
            List<Note> items;
            if (folderId == -1)
                items = await App.Database.GetAllNotesAsync();
            else
                items = await App.Database.GetNotesAsync(folderId);
            foreach (Note note in items.ToList())
            {
                string f = note.Text + " " + note.Name;
                if (!(f.ToLower().Contains(text)))
                {
                    items.Remove(note);
                }
            }
            MyListView.ItemsSource = items;

        }

        private async void searchBtn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(searchBtn.Text))
            {
                if (folderId == -1)
                    MyListView.ItemsSource = await App.Database.GetAllNotesAsync();
                else
                    MyListView.ItemsSource = await App.Database.GetNotesAsync(folderId);
            }
        }*/

        private void toolBtn_Clicked(object sender, EventArgs e)
        {
            popup.IsOpen = true;
        }

        private async void searchBtn_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SearchPage));
        }
    }
}
