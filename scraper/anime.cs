using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scraper
{
    internal class anime
    {
        public string Name { get; set; }
        public string Story { get; set; }
        public string ImageUrl { get; set; }
        public List<string> EpisodeUrl { get; set; }
        public Dictionary<string, List<string>> EpisodeServers { get; set; }
    } 
}
