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
    class NumbersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        

        public NumbersViewModel()
        {
            NumberCommand = new Command<string>(
                execute:(string arg) =>
                {
                    Number += arg;
                    Count++;
                },
                canExecute: (string arg) =>
                {
                    return !(arg.Length == 5);
                });
            BackSpaceCommand = new Command(
                execute: () =>
            {
                if (Number != "")
                {
                    Number = Number.Substring(0, Number.Length - 1);
                    Count--;
                }
            });
            ClearCommand = new Command(execute: () =>
            {
                Number = "";
                Count = 0;
            });
        }

        public ICommand NumberCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackSpaceCommand { get; }
        string number = String.Empty;
        int count = 0;
        public int Count
        {
            private set
            {
                if (count != value)
                {
                    count = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                }
            }
            get
            {
                return count;
            }
        }
        public string Number
        {
            private set
            {
                if (number != value)
                {
                    number = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Number"));
                }
            }
            get
            {
                return number;
            }
        }

    }
}
