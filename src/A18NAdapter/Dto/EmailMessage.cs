using System;

namespace A18NAdapter.Dto
{
    public class EmailMessage
    {
        public string MessageId { get; set; }
        public string ProfileId { get; set; }
        public string ToAddress { get; set; }
        public string FromAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}