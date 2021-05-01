using System;

namespace Snoop.Client
{
    public class MessageQueryModel
    {
        public long Id { get; set; }
        public byte[] Headers { get; set; }
        public byte[] Body { get; set; }
        public int Priority { get; set; }
        public DateTimeOffset? Visible { get; set; }
        public DateTimeOffset? Expiration { get; set; }
    }
}
