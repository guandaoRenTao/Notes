using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Notes.Views;
using Telerik.XamarinForms.RichTextEditor;
namespace Notes.Helpers
{
    public class DrawingToolbarItem : AddImageToolbarItem ,ICommand 
    {
        public string Icon { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        
        public async void Execute(object parameter)
        {
            await Shell.Current.GoToAsync($"{nameof(SettingsPage)}");
            //throw new NotImplementedException();
        }
    }
}
