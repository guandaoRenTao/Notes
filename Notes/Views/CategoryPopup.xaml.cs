using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using Notes.Models;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using System.IO;
using System.Reflection;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CategoryPopup : PopupPage
    {

        CollectionView list;
        public CategoryPopup()
        {
            InitializeComponent();
            initEmojiList();
            BindingContext = new Category();
        }
        public CategoryPopup(CollectionView collectionView)
        {
            InitializeComponent();
            list = collectionView;
        }
        public CategoryPopup(Category folder)
        {
            InitializeComponent();
            initEmojiList();
            BindingContext = folder;
            title.Text = "Изменение категории";
        }
        void initEmojiList()
        {

            /*var group1 = new EmojiGroup("first group", new List<Emj>());
            for (int i = 128512; i < 129488; i++)
            {
                string emo = new Emoji(i).ToString();
                group1.Add(new Emj(emo,i));
            }
            var group2 = new EmojiGroup("second group", new List<Emj>());
            for (int i = 129510; i > 126980; i--)
            {
                string emo = new Emoji(i).ToString();
                group2.Add(new Emj(emo, i ));
            }
            Emojis.Add(group1);
            Emojis.Add(group2);*/
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(CategoryPopup)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Notes.Data.emojis.txt");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            string[] s = text.Split(' ');
            emojiList.ItemsSource = s.ToList();
        }
        protected override void OnAppearing()
        {
            App.Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
            base.OnAppearing();
        }
        private async void okBtn_Clicked(object sender, EventArgs e)
        {
            Category folder = (Category)BindingContext;
            folder.Name = name.Text;
            List<Note> notes = await App.Database.GetNotesAsync(folder.ID);
            foreach(var note in notes)
            {
                await App.Database.SaveNoteAsync(note);
            }
            if (folder.Date == DateTime.MinValue)
                folder.Date = DateTime.Now;
            await App.Database.SaveCategoryAsync(folder);
            var c = (AppShell)Shell.Current;
            //c.Items.Clear();
            Device.BeginInvokeOnMainThread(() => c.init(true));
            c.folders.SelectedItem = folder;
            //list.ItemsSource = await App.Database.GetCategorysAsync();
            await PopupNavigation.Instance.PopAsync();
        }

        private async void deleteBtn_Pressed(object sender, EventArgs e)
        {
            Category folder = (Category)BindingContext;
            bool answer = await DisplayAlert("Удаление", "Все заметки из категории также будут удалены. Удалить? ", "Да", "Отмена");
            if (answer)
            {
                await App.Database.DeleteCategoryAsync(folder);
                var a = await App.Database.GetNotesAsync(folder.ID);
                foreach (Note note in a)
                    await App.Database.DeleteNoteAsync(note);
                var c = (AppShell)Shell.Current;
                Device.BeginInvokeOnMainThread(() => c.init(true));
                await PopupNavigation.Instance.PopAsync();
            }

        }


        private void emojiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void emojiPicker_Clicked(object sender, EventArgs e)
        {
            emojiPopup.IsOpen = true;
        }

        private void ok_Pressed(object sender, EventArgs e)
        {
            string icon = (string)emojiList.SelectedItem;
            Category folder = (Category)BindingContext;
            folder.Icon = icon;
            OnPropertyChanged(nameof(folder.Icon));
            emojiPopup.IsOpen = false;
            emojiPicker.Text = icon;
        }
    }


   
}