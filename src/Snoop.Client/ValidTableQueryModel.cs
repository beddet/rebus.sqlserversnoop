namespace Snoop.Client
{
    public class ValidTableQueryModel
    {
        public long Id { get; set; }
        public byte[] Headers { get; set; }
        public byte[] Body { get; set; }
        public int Count { get; set; }
    }
}