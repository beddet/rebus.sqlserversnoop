using System;
using System.Collections.Generic;

namespace Snoop.Client
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public List<MessageHeaderViewModel> Headers { get; set; }
        public string Type { get; set; }
        public string ReturnAddress { get; set; }
        public DateTime SentTime { get; set; }
        public string Body { get; set; }
        public string ErrorDetails { get; set; }

        public MessageViewModel(int id, int size, List<MessageHeaderViewModel> headers, string type, string returnAddress, DateTime sentTime, string body, string errorDetails)
        {
            Id = id;
            Size = size;
            Headers = headers ?? new List<MessageHeaderViewModel>();
            Type = type;
            ReturnAddress = returnAddress;
            SentTime = sentTime;
            Body = body;
            ErrorDetails = errorDetails;
        }
    }
}