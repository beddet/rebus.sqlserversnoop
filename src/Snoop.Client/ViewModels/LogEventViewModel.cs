using System;
using Serilog.Events;

namespace Snoop.Client.ViewModels
{
    public class LogEventViewModel
    {
        public DateTime Timestamp { get; }
        public string Message { get; }
        public string Level { get; }

        public LogEventViewModel(LogEvent logEvent)
        {
            Timestamp = logEvent.Timestamp.DateTime;
            Message = logEvent.RenderMessage();
            Level = logEvent.Level.ToString();
        }
    }
}