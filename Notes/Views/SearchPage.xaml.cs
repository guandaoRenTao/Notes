using Notes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public List<Note> Notes;
        List<Note> _items;
        public List<Note> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged(); }
        }
        bool isClearVisible = false;
        public bool IsClearVisible
        {
            get { return isClearVisible; }
            set { isClearVisible = value; OnPropertyChanged(); }
        }

        // public Command SearchCommand { get; private set; }
         public Command ClearCommand { get; private set; }
        public SearchPage()
        {
            InitializeComponent();
            ClearCommand = new Command(
               execute: () =>
               {
                   searchBar.Text = "";
                   searchBar.Focus();
                   IsClearVisible = false;
               });
            BindingContext = this;
            
        }
        async void init()
        {
            IsBusy = true;
            MyListView.SelectedItems.Clear();
            var notes = await App.Database.GetAllNotesAsync();
            Notes = new List<Note>(notes.Reverse<Note>());
            Items = Notes;
            search();
            IsBusy = false;
        }
        protected async override void OnAppearing()
        {
            init();
            base.OnAppearing();
            await MyListView.FadeTo(1);
            searchBar.Focus();
        }
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();
            await MyListView.FadeTo(0);
        }
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsBusy = true;
            if (e.CurrentSelection.Count != 0)
            {
                Note note = (Note)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"{nameof(NoteEntryPage)}?{nameof(NoteEntryPage.ItemId)}={note.ID}");
            }
            IsBusy = false;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            search();
        }
        void search()
        {
            IsBusy = true;
            if (!String.IsNullOrEmpty(searchBar.Text))
            {
                IsClearVisible = true;
                string text = searchBar.Text.ToLower();
                Items = Notes.Where(a => (a.Text + " " + a.Name).ToLower().Contains(text)).ToList();
                /*foreach (var note in Notes)
                {
                    string f = note.Text + " " + note.Name;
                    if (!(f.ToLower().Contains(text)))
                    {
                        Items.Remove(note);
                    }
                }*/
            }
            else
            {
                IsClearVisible = false;
                Items = Notes;
            }
            IsBusy = false;
        }
    }
}
