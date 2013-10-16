using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public class FeedItem
    {
        public string Creator { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime Published { get; set; }
        public List<string> Tags { get; set; }
        public FeedItem()
        {
            this.Tags = new List<string>();
        }
    }
}