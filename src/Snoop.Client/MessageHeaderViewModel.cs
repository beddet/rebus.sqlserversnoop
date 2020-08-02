namespace Snoop.Client
{
    public class MessageHeaderViewModel
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public MessageHeaderViewModel(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}