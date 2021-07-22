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

        public DelegateCommand<TableViewModel> RemoveCommand { get; set; }
        public DelegateCommand<TableViewModel> ReloadMessagesCommand { get; set; }
        public DelegateCommand<TableViewModel> PurgeCommand { get; set; }
        public DelegateCommand<TableViewModel> ReturnAllToSourceQueueCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand ReloadConnectionCommand { get; }
        
        public ConnectionViewModel()
        {
            Tables = new ObservableCollection<TableViewModel>();
            RemoveCommand = new DelegateCommand<TableViewModel>(Remove);
            ReloadMessagesCommand = new DelegateCommand<TableViewModel>(ReloadMessages);
            PurgeCommand = new DelegateCommand<TableViewModel>(PurgeMessages);
            ReturnAllToSourceQueueCommand = new DelegateCommand<TableViewModel>(ReturnAllMessagesToSourceQueue);
            EditCommand = new DelegateCommand(Edit);
            SaveCommand = new DelegateCommand(Save);
            ReloadConnectionCommand = new DelegateCommand(ReloadConnection);

            _rebusService = new RebusService();
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
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
                if (SetProperty(ref _selectedTable, value)) TryGetMessages();
            }
        }

        private int _numberOfMessages;
        [UsedImplicitly]
        public int NumberOfMessages
        {
            get => _numberOfMessages;
            set => SetProperty(ref _numberOfMessages, value);
        }

        private string _connectionString;
        public string ConnectionString
        {
            get => _connectionString;
            set => SetProperty(ref _connectionString, value);
        }

        private void Save()
        {
            IsEditing = false;
            LoadTables();
        }

        private void Edit()
        {
            IsEditing = true;
        }

        public async void LoadTables(bool forceRefresh = false)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString)) return;
            if (Tables.Any() && !forceRefresh) return;
            
            var tables = await _rebusService.GetValidTables(ConnectionString);
            Tables = new ObservableCollection<TableViewModel>(tables);
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

        private void ReloadConnection()
        {
            LoadTables(true);
        }
    }
}
