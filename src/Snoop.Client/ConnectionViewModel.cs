using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Mvvm;

namespace Snoop.Client
{
    public class ConnectionViewModel : BindableBase
    {
        private readonly RebusService _rebusService;

        private string _connectionString;
        private int _numberOfMessages;
        private TableViewModel _selectedTable;
        private ObservableCollection<TableViewModel> _tables;

        public DelegateCommand<TableViewModel> RemoveCommand { get; set; }
        public DelegateCommand<TableViewModel> ReloadMessagesCommand { get; set; }
        public DelegateCommand<TableViewModel> PurgeCommand { get; set; }
        public DelegateCommand<TableViewModel> ReturnAllToSourceQueueCommand { get; set; }

        public ObservableCollection<TableViewModel> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public TableViewModel SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (SetProperty(ref _selectedTable, value)) TryGetMessages();
            }
        }

        [UsedImplicitly]
        public int NumberOfMessages
        {
            get => _numberOfMessages;
            set => SetProperty(ref _numberOfMessages, value);
        }

        public string ConnectionString
        {
            get => _connectionString;
            set => SetProperty(ref _connectionString, value);
        }

        public ConnectionViewModel()
        {
            Tables = new ObservableCollection<TableViewModel>();
            RemoveCommand = new DelegateCommand<TableViewModel>(Remove);
            ReloadMessagesCommand = new DelegateCommand<TableViewModel>(ReloadMessages);
            PurgeCommand = new DelegateCommand<TableViewModel>(PurgeMessages);
            ReturnAllToSourceQueueCommand = new DelegateCommand<TableViewModel>(ReturnAllMessagesToSourceQueue);
            _rebusService = new RebusService();
        }

        public void LoadTables()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString)) return;
            if (Tables.Any()) return;

            Tables = new ObservableCollection<TableViewModel>(_rebusService.GetValidTables(ConnectionString));
        }

        private async void ReturnAllMessagesToSourceQueue(TableViewModel obj)
        {
            await obj.ReturnAllToSourceQueue();
        }

        private void PurgeMessages(TableViewModel obj)
        {
            obj.Purge();
        }

        private void ReloadMessages(TableViewModel obj)
        {
            obj.ReloadMessages();
        }

        private void Remove(TableViewModel tableViewModel)
        {
            Tables.Remove(tableViewModel);
            SelectedTable = null;
        }

        private void TryGetMessages()
        {
            SelectedTable?.LoadMessages();
        }
    }
}
