using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public class Feed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Language { get; set; }
        public List<FeedItem> Items { get; set; }
        public Feed()
        {
            this.Items = new List<FeedItem>();
        }
    }
}