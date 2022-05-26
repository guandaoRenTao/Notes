using System;
using System.IO;
using Notes.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Telerik.XamarinForms.RichTextEditor;
using Notes.Controls;
using Plugin.Media;
using Notes.Helpers;
using Xamarin.Essentials;
using Xamarin.CommunityToolkit.Effects;
using Telerik.XamarinForms.Common;
using System.Text.RegularExpressions;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]
    public partial class NoteEntryPage : ContentPage
    {
        public interface ICustomNotification
        {
            void send(string title, string message);
        }
        ICustomNotification notification;
        public string noteTitle;
        public string noteContent;
        public string NoteTitle
        {
            get { return noteTitle; }
            set { noteTitle = value; OnPropertyChanged(); }
        }
        public string NoteContent
        {
            get { return noteContent; }
            set { noteContent = value; OnPropertyChanged(); }
        }

        public List<Category> CategoryList;
        public string ItemId
        {
            set
            {
                LoadNote(value);
            }
        }
        public int CategoryId
        {
            set
            {
                folderId = value;
            }
        }
        int folderId;
        async void LoadNote(string itemId)
        {
            try
            {
                int id = Convert.ToInt32(itemId);
                // Retrieve the note and set it as the BindingContext of the page.
                Note note = await App.Database.GetNoteAsync(id);
                folderId = note.CategoryID;
                BindingContext = note;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load note.");
            }
        }


        public Command DrawingCommand { get; private set; }
        /*public Command ShareCommand { get; private set; }
        public Command ReminderCommand { get; private set; }
        public Command ChangeCategoryCommand { get; private set; }*/

        public NoteEntryPage()
        {
            InitializeComponent();
            TelerikLocalizationManager.Manager = new CustomTelerikLocalizationManager();
            BindingContext = new Note() { CategoryID = folderId };
            notification = DependencyService.Get<ICustomNotification>();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            init();
            CategoryList = await App.Database.GetCategoriesAsync();
            App.Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            App.Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
        }
        public bool flag = false;
        public string filename = "";

        void init()
        {
            DrawingCommand = new Command(
               execute: async () =>
               {
                   MessagingCenter.Subscribe<PaintingPage, string>(this, "image", async (sender, arg) =>
                   {
                       flag = true;
                       filename = Path.Combine("/storage/emulated/0/Pictures/FingerPaint", arg);
                       //await DisplayAlert("Получено изображение", filename, "ok");
                       imagePopup.IsOpen = true;
                       /*TouchAction touchAction = new TouchAction(driver);
                       touchAction.Tap(element).Perform();*/

                       //text_PickImage(text, new PickImageEventArgs());
                   });
                   await PopupNavigation.Instance.PushAsync(new PaintingPage());
               });
           /* ShareCommand = new Command(
               execute: () =>
               {
                   popup.IsOpen = false;
                   share_Clicked(this, EventArgs.Empty);
               });
            ReminderCommand = new Command(
               execute: () =>
               {
                   popup.IsOpen = false;
               });
            ChangeCategoryCommand = new Command(
               execute: () =>
               {
                   popup.IsOpen = false;
                   categoryPopup.IsOpen = true;
                   foldersList.ItemsSource = CategoryList;
               });*/
        }



        private void reminderTapped(object sender, EventArgs e)
        {
            getText();
            notifyPopup.IsOpen = true;
            popup.IsOpen = false;
        }
        private void shareTapped(object sender, EventArgs e)
        {
            popup.IsOpen = false;
            share_Clicked(this, EventArgs.Empty);
        }
        private void moveTapped(object sender, EventArgs e)
        {
            popup.IsOpen = false;
            categoryPopup.IsOpen = true;
            foldersList.ItemsSource = CategoryList;
        }


        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Удаление", "Удалить заметку?", "Да", "Отмена");
            if (answer)
            {
                var note = (Note)BindingContext;

                await App.Database.DeleteNoteAsync(note);
                // Navigate backwards
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void okBtn_Clicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            if (note.Date == DateTime.MinValue)
                note.Date = DateTime.Now;
            if (string.IsNullOrWhiteSpace(note.Name))
            {
                note.Name = "";
            }
            note.CategoryID = folderId;
            if (folderId != -1)
            {
                Category folder = await App.Database.GetCategoryAsync(folderId);
            }
            note.Text = await this.text.GetHtmlAsync();
            await App.Database.SaveNoteAsync(note);
            await Shell.Current.GoToAsync("..");
        }

        private void settingsBtn_Clicked(object sender, EventArgs e)
        {

        }

        private void header_TextChanged(object sender, TextChangedEventArgs e)
        {
            var note = (Note)BindingContext;
            if (note.Date == DateTime.MinValue)
                note.Date = DateTime.Now;
            if (string.IsNullOrWhiteSpace(note.Name))
            {
                note.Name = "";
            }
            
        }

        private async void text_PickImage(object sender, PickImageEventArgs e)
        {
            if (flag)
            {
                var imageSource = RichTextImageSource.FromFile(filename);
                e.Accept(imageSource);
                flag = false;
                imagePopup.IsOpen = false;
                return;
            }
            else
            {
                var mediaPlugin = CrossMedia.Current;

                if (mediaPlugin.IsPickPhotoSupported)
                {
                    if (!await PermissionsHelper.RequestPhotosAccess())
                    {
                        return;
                    }

                    if (!await PermissionsHelper.RequestStorrageAccess())
                    {
                        return;
                    }

                    var mediaFile = await mediaPlugin.PickPhotoAsync();

                    if (mediaFile != null)
                    {
                        var imageSource = RichTextImageSource.FromFile(mediaFile.Path);
                        e.Accept(imageSource);
                        return;
                    }
                }
            }
        }

        async void getText()
        {
            string text = await this.text.GetHtmlAsync();
            Regex regex1 = new Regex(@"<.*?>");
            text = regex1.Replace(text, "");
            NoteContent = text;
            NoteTitle = header.Text;
            Device.BeginInvokeOnMainThread((Action)(() => {
                reminderMessage.Text = NoteContent;
                reminderTitle.Text = NoteTitle;
            }));
        }
        private async void share_Clicked(object sender, EventArgs e)
        {
            getText();
            await Share.RequestAsync(new ShareTextRequest()
            {
                Text = "'" + header.Text + "'\n" + NoteContent
            });
        }

        private void toolBtn_Clicked(object sender, EventArgs e)
        {
            popup.IsOpen = true;
        }

        private void foldersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count != 0)
            {
                Category folder = (Category)e.CurrentSelection.FirstOrDefault();
                CategoryId = folder.ID; 
                categoryPopup.IsOpen = false;
            }

        }

        private void dontPastePictureBtn_Clicked(object sender, EventArgs e)
        {
            flag = false;
            imagePopup.IsOpen = false;
        }

        void OnScheduleClick(object sender, EventArgs e)
        {
            /*string title = reminderTitle.Text;
            string message = reminderMessage.Text;
            DateTime date = reminderDate.Date + reminderTime.Time;
            */
            //notification.send("Title", "message");
            notifyPopup.IsOpen = false;
        }

    }
    
}