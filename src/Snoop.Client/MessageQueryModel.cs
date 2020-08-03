namespace Snoop.Client
{
    public class MessageQueryModel
    {
        public long Id { get; set; }
        public byte[] Headers { get; set; }
        public byte[] Body { get; set; }
    }
}