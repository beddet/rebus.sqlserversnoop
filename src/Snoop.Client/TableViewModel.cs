﻿using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;

namespace Snoop.Client
{
    public class TableViewModel : BindableBase
    {
        private readonly RebusService _rebusService;
        public TableViewModel(string name)
        {
            Name = name;
            Messages = new ObservableCollection<MessageViewModel>();
            _rebusService = new RebusService();
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

        public void LoadMessages(string connectionString)
        {
            if (Messages.Any()) return;
            ReloadMessages(connectionString);
        }

        private void ReloadMessages(string connectionString)
        {
            Messages = new ObservableCollection<MessageViewModel>(_rebusService.GetMessages(connectionString, Name));
        }
    }
}