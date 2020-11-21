using System;
using System.Collections.Generic;

namespace SuperGamblino.Messages
{
    public class Message
    {
        public Message()
        {
            Fields = new List<Field>();
        }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public List<Field> Fields { get; set; }
        public string Footer { get; set; }

        public event EventHandler OnUpdate;
        public event EventHandler OnDelete;

        /// <summary>
        ///     Updates message via event when its set up
        /// </summary>
        public void Update()
        {
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Deletes message via event when its set up
        /// </summary>
        public void Delete()
        {
            OnDelete?.Invoke(this, EventArgs.Empty);
        }


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