﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;

namespace Snoop.Client
{
    public class TableViewModel : BindableBase
    {
        private readonly string _connectionString;
        public DelegateCommand<MessageViewModel> DeleteMessageCommand { get; set; }
        public DelegateCommand<MessageViewModel> ReturnToSourceQueueCommand { get; set; }

        private readonly RebusService _rebusService;
        public TableViewModel(string name, int messageCount, string connectionString)
        {
            _connectionString = connectionString;
            Name = name;
            MessageCount = messageCount;
            Messages = new ObservableCollection<MessageViewModel>();
            _rebusService = new RebusService();
            DeleteMessageCommand = new DelegateCommand<MessageViewModel>(DeleteMessage);
            ReturnToSourceQueueCommand = new DelegateCommand<MessageViewModel>(ReturnToSourceQueue);
        }

        private void DeleteMessage(MessageViewModel messageViewModel)
        {
            if (SelectedMessage == messageViewModel)
            {
                SelectedMessage = null;
            }
            _rebusService.DeleteMessage(_connectionString, Name, messageViewModel.Id);
            Messages.Remove(messageViewModel);
            MessageCount--;
        }

        private void ReturnToSourceQueue(MessageViewModel messageViewModel)
        {
            if (Name == messageViewModel.SourceQueue) return;

            if (SelectedMessage == messageViewModel)
            {
                SelectedMessage = null;
            }
            _rebusService.ReturnToSourceQueue(_connectionString, Name, messageViewModel.SourceQueue, messageViewModel);
            Messages.Remove(messageViewModel);
            MessageCount--;
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

        public void LoadMessages()
        {
            if (Messages.Any()) return;
            ReloadMessages();
        }

        private int _messageCount;
        public int MessageCount
        {
            get => _messageCount;
            set => SetProperty(ref _messageCount, value);
        }

        public void ReloadMessages()
        {
            Messages = new ObservableCollection<MessageViewModel>(_rebusService.GetMessages(_connectionString, Name));
            MessageCount = Messages.Count;
        }

        private MessageViewModel _selectedMessage;
        public MessageViewModel SelectedMessage
        {
            get => _selectedMessage;
            set => SetProperty(ref _selectedMessage, value);
        }

        public void Purge()
        {
            _rebusService.Purge(_connectionString, Name);
            ReloadMessages();
        }

        public async Task ReturnAllToSourceQueue()
        {
            LoadMessages();

            var messages = Messages.ToList();

            int messageCount = messages.Count;

            var task = Task.Factory.StartNew(() =>
                {
                    foreach (var messageViewModel in messages)
                    {
                        _rebusService.ReturnToSourceQueue(_connectionString, Name, messageViewModel.SourceQueue, messageViewModel);
                        messageCount--;
                    }
                },
                TaskCreationOptions.LongRunning);

            while (!task.IsCompleted)
            {
                MessageCount = messageCount;
                await Task.Delay(1000);
            }

            await task;

            ReloadMessages();
        }
    }
}
