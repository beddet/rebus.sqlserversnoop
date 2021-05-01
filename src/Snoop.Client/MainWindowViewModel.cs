using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;

namespace Snoop.Client
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand<ConnectionViewModel> DeleteCommand { get; set; }
        public DelegateCommand OnLoadedCommand { get; }

        public MainWindowViewModel()
        {
            Connections = new ObservableCollection<ConnectionViewModel>();
            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand<ConnectionViewModel>(Delete);
            OnLoadedCommand = new DelegateCommand(OnLoaded);
        }

        private void OnLoaded()
        {
            if (!Connections.Any())
                Add();
        }

        private void Add()
        {
            var connectionViewModel = new ConnectionViewModel
            {
                IsEditing = true
            };
            Connections.Add(connectionViewModel);
            SelectedConnection = connectionViewModel;
        }

        private void Delete(ConnectionViewModel connectionViewModel)
        {
            Connections.Remove(connectionViewModel);
        }

        private ObservableCollection<ConnectionViewModel> _connections;
        public ObservableCollection<ConnectionViewModel> Connections
        {
            get => _connections;
            set => SetProperty(ref _connections, value);
        }

        private ConnectionViewModel _selectedConnection;
        public ConnectionViewModel SelectedConnection
        {
            get => _selectedConnection;
            set => SetProperty(ref _selectedConnection, value);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedConnection))
                SelectedConnection?.LoadTables();
        }
    }
}
