using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Snoop.Client
{
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
}