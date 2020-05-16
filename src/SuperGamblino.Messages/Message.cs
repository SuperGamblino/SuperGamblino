using System.Collections.Generic;

namespace SuperGamblino.Messages
{
    public class Message
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
        public string Footer { get; set; }

        public Message AddField(string key, string value)
        {
            Fields.Add(new Field(key, value));
            return this;
        }

        public Message WithFooter(string value)
        {
            Footer = value;
            return this;
        }
    }
}