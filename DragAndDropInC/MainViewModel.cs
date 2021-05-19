using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace DragAndDropInC
{
   
    public class Printer
    {
        public string fullname { get; set; }
        public string comment { get; set; }
        public PrintTicket defaultPrintTicket { get; set; }
        public string description { get; set; }
        public bool isInError { get; set; }
        public bool isOffline {get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        #region [INotifyPropertyChanged]
        public event PropertyChangedEventHandler PropertyChanged;

        // using "virtual" here results in call chain violation CA2214
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public MainViewModel()
        {
            _canExecute = true;
            Printers = new ObservableCollection<Printer>();

            RefreshPrinterList();
        }

        public ObservableCollection<Printer> Printers { get; set; }


        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new CommandHandler(() => RefreshPrinterList(), _canExecute));
            }
        }

        private bool _canExecute;

        public void RefreshPrinterList()
        {
            EnumeratedPrintQueueTypes[] enumerationFlags = {EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections};

            var LocalPrintServer  = new PrintServer();           
            var printQueues = LocalPrintServer.GetPrintQueues(enumerationFlags);

            foreach (PrintQueue p in printQueues)
            {
                Printers.Add(new Printer { comment = p.Comment, defaultPrintTicket = p.DefaultPrintTicket, description = p.Description, 
                                           fullname = p.FullName, isInError = p.IsInError, isOffline = p.IsOffline });
            }

        }

        public class CommandHandler : ICommand
        {
            private Action _action;
            private bool _canExecute;
            public CommandHandler(Action action, bool canExecute)
            {
                _action = action;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                _action();
            }
        }
    }
}
