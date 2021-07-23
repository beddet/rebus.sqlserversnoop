using System.Collections.ObjectModel;
using Prism.Mvvm;
using Serilog.Core;
using Serilog.Events;

namespace Snoop.Client.ViewModels
{
    public class LoggerViewModel : BindableBase, ILogEventSink
    {
        public LoggerViewModel()
        {
            Events = new ObservableCollection<LogEventViewModel>();
        }

        private ObservableCollection<LogEventViewModel> _events;
        public ObservableCollection<LogEventViewModel> Events
        {
            get => _events;
            set => SetProperty(ref _events, value);
        }

        private int _numberOfEvents;
        public int NumberOfEvents
        {
            get => _numberOfEvents;
            set => SetProperty(ref _numberOfEvents, value);
        }

        public void Emit(LogEvent logEvent)
        {
            Events.Add(new LogEventViewModel(logEvent));
            NumberOfEvents++;
        }
    }
}