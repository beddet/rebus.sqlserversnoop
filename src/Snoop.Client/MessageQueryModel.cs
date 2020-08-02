namespace Snoop.Client
{
    public class MessageQueryModel
    {
        public int Id { get; set; }
        public byte[] Headers { get; set; }
        public byte[] Body { get; set; }
    }
}