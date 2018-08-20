using System;

namespace WALLE.Link.Dto
{
    public class Event
    {
        public string Id { get; set; }

        public DateTime CreationTime { get; set; }

        public string Sender { get; set; }

        public string ContentType { get; set; }

        public string Content { get; set; }
    }
}
