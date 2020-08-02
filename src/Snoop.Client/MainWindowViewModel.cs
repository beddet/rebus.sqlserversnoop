using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace Snoop.Client
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand AddCommand { get; set; }

        public MainWindowViewModel()
        {
            Connections = new ObservableCollection<ConnectionViewModel>();
            AddCommand = new DelegateCommand(() => Add());
            Add();
        }

        private void Add()
        {
            var connectionViewModel = new ConnectionViewModel();
            Connections.Add(connectionViewModel);
            SelectedConnection = connectionViewModel;
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
    }
}