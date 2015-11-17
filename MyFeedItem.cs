using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Windows.Data;
using System.Xml;

namespace Volos_Fans
{
    public class MyFeedItem 
    {
        public string Title { get; set; }
        public Uri Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
    }
}
       