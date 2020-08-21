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
        public DateTimeOffset SentTime { get; set; }
        public string Body { get; set; }
        public string ErrorDetails { get; set; }

        public MessageViewModel(long id, List<MessageHeaderViewModel> headers, string type, string sourceQueue, DateTimeOffset sentTime, string body, string errorDetails)
        {
            Id = id;
            Headers = headers ?? new List<MessageHeaderViewModel>();
            Type = type;
            SourceQueue = sourceQueue;
            SentTime = sentTime;
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