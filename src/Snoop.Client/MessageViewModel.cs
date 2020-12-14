using System;
using System.Collections.Generic;

namespace Snoop.Client
{
    public class MessageViewModel
    {
        public long Id { get; set; }
        public List<MessageHeaderViewModel> Headers { get; set; }
        public string Type { get; set; }
        public string SourceQueue { get; set; }
        public DateTime SentTime { get; set; }
        public DateTime? VisibleFrom { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Body { get; set; }
        public string ErrorDetails { get; set; }

        public MessageViewModel(long id, List<MessageHeaderViewModel> headers, string type, string sourceQueue, DateTimeOffset sentTime, DateTimeOffset? visibleFrom, DateTimeOffset? expiresAt, string body, string errorDetails)
        {
            Id = id;
            Headers = headers ?? new List<MessageHeaderViewModel>();
            Type = type;
            SourceQueue = sourceQueue;
            SentTime = sentTime.LocalDateTime;
            VisibleFrom = visibleFrom?.LocalDateTime;
            ExpiresAt = expiresAt?.LocalDateTime;
            Body = body;
            ErrorDetails = errorDetails;
        }

        public MessageViewModel()
        {
            
        }

        private MessageViewModel(string body)
        {
            Body = body;
            Headers = new List<MessageHeaderViewModel>();
        }

        public static MessageViewModel Error(string error)
        {
            return new MessageViewModel(error);
        }
    }
}
