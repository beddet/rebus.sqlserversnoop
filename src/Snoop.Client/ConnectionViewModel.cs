﻿using System;
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
        public DelegateCommand<TableViewModel> ReloadMessagesCommand { get; set; }
        public DelegateCommand<TableViewModel> PurgeCommand { get; set; }
        public DelegateCommand<TableViewModel> ReturnAllToSourceQueueCommand { get; set; }

        private RebusService _rebusService;

        public ConnectionViewModel()
        {
            Tables = new ObservableCollection<TableViewModel>();
            RemoveCommand = new DelegateCommand<TableViewModel>(x => Remove(x));
            ReloadMessagesCommand = new DelegateCommand<TableViewModel>(ReloadMessages);
            PurgeCommand = new DelegateCommand<TableViewModel>(PurgeMessages);
            ReturnAllToSourceQueueCommand = new DelegateCommand<TableViewModel>(ReturnAllMessagesToSourceQueue);
            _rebusService = new RebusService();
        }

        private void ReturnAllMessagesToSourceQueue(TableViewModel obj)
        {
            obj.ReturnAllToSourceQueue();
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

            SelectedTable.LoadMessages();
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
