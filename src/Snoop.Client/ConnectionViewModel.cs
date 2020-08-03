using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Prism.Commands;
using Prism.Mvvm;

namespace Snoop.Client
{
    public class ConnectionViewModel : BindableBase
    {
        public DelegateCommand<TableViewModel> RemoveCommand { get; set; }
        private RebusService _rebusService;

        public ConnectionViewModel()
        {
            Tables = new ObservableCollection<TableViewModel>();
            RemoveCommand = new DelegateCommand<TableViewModel>(x => Remove(x));
            _rebusService = new RebusService();
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

        private int _numberOfMessages;
        public int NumberOfMessages
        {
            get => _numberOfMessages;
            set => SetProperty(ref _numberOfMessages, value);
        }

        private void TryGetMessages()
        {
            if (SelectedTable is null) return;

            SelectedTable.LoadMessages(ConnectionString);
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
            if (string.IsNullOrWhiteSpace(ConnectionString)) return;
            if (Tables.Any()) return;

            try
            {
                Tables = new ObservableCollection<TableViewModel>(_rebusService.GetValidTables(ConnectionString));
            }
            catch (Exception e)
            {
                //todo handle errors
            }
        }
    }
}