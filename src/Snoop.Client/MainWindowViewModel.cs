using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
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

    public class MessageViewModel : BindableBase
    {

    }

    public class TableViewModel : BindableBase
    {
        public TableViewModel(string name)
        {
            Name = name;
            Messages = new ObservableCollection<MessageViewModel>();
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private ObservableCollection<MessageViewModel> _messages;
        public ObservableCollection<MessageViewModel> Messages
        {
            get => _messages;
            set => SetProperty(ref _messages, value);
        }
    }
    
    public class ConnectionViewModel : BindableBase
    {
        public DelegateCommand<TableViewModel> RemoveCommand { get; set; }

        public ConnectionViewModel()
        {
            Tables = new ObservableCollection<TableViewModel>();
            RemoveCommand = new DelegateCommand<TableViewModel>(x => Remove(x));
        }

        private void Remove(TableViewModel tableViewModel)
        {
            Tables.Remove(tableViewModel);
            SelectedTable = null;
        }

        private ObservableCollection<TableViewModel> _tables;
        public ObservableCollection<TableViewModel> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        private TableViewModel _selectedTable;
        public TableViewModel SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (SetProperty(ref _selectedTable, value))
                {
                    TryGetMessages();
                }
            }
        }

        private void TryGetMessages()
        {
            if (SelectedTable is null) return;
           //todo implement
        }

        private string _connectionString;
        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                if (SetProperty(ref _connectionString, value))
                {
                    TryConnect();
                }
            }
        }

        private void TryConnect()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ConnectionString)) return;

                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    var tables = connection.Query<string>("select name from sys.tables").ToList();
                    Tables = new ObservableCollection<TableViewModel>(tables.Select(x => new TableViewModel(x)));
                }
            }
            catch (Exception e)
            {
                //todo handle errors
            }
        }
    }
}