using HtmlAgilityPack;
using scraper;
using CsvHelper;
using System;
using System.Globalization;
using static System.Reflection.Metadata.BlobBuilder;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

internal class Program
{
    private static void Main(string[] args)

    {

        /*
           If i will get the data from all pages i will use for loop: 
           string url = $"https://animelek.me/%D9%82%D8%A7%D8%A6%D9%85%D8%A9-%D8%A7%D9%84%D8%A3%D9%86%D9%85%D9%8A/{i}";
        */

        //Get the URL of the website
        //for (int i = 1; i <= 49; i++)
        //{

        string url = "https://animelek.me/%D9%82%D8%A7%D8%A6%D9%85%D8%A9-%D8%A7%D9%84%D8%A3%D9%86%D9%85%D9%8A/";
            var links = GetAnimeLinks(url);
            List<anime> Anime = GetAnime(links);
            Export(Anime);
        //}

    }
    static void Export(List<anime> animes)
    {
        var animeList = new List<Dictionary<string, object>>();

        foreach (var anime in animes)
        {
            var animeData = new Dictionary<string, object>
        {
            { "Name", anime.Name },
            { "Story", anime.Story },
            { "ImageUrl", anime.ImageUrl }
        };

            if (anime.EpisodeServers != null && anime.EpisodeServers.Count > 0)
            {
                // Create a list to hold episodes
                var episodes = new List<Dictionary<string, object>>();

                foreach (var episodeEntry in anime.EpisodeServers)
                {
                    var episodeData = new Dictionary<string, object>
                {
                    { "EpisodeUrl", episodeEntry.Key },
                    { "ServersUrl", episodeEntry.Value }
                };

                    episodes.Add(episodeData);
                }

                if (episodes.Count > 0)
                {
                    animeData.Add("Episodes", episodes);
                }
            }

            animeList.Add(animeData);
        }

        // Serialize the list of anime objects to JSON format
        string jsonString = JsonSerializer.Serialize(animeList, new JsonSerializerOptions
        {
            WriteIndented = true // Makes the JSON output nicely formatted with indentation
        });

        // Save the JSON string to a file
        string filePath = "C:\\Users\\mones\\Desktop\\animes.json"; 
        File.WriteAllText(filePath, jsonString);
    }


    static List<anime> GetAnime(List<string> links)
    {
        var animes = new List<anime>();
        foreach (var link in links)
        {
            var doc = GetDocument(link);
            var Anime = new anime();
            // get name of anime
            HtmlNode Node = doc.DocumentNode.SelectSingleNode("//h1[@class=\"anime-details-title\"]");
            if (Node != null) // check if anime exisit or not 
            {
                Anime.Name = Node.InnerText;
                // get the story of anime
                var xpath = "//*[@class=\"anime-story\"]";
                Anime.Story = doc.DocumentNode.SelectSingleNode(xpath).InnerText;
                // get the image URL 
                Anime.ImageUrl = doc.DocumentNode.SelectSingleNode("//*[@class=\"anime-thumbnail-pic\"]/img").Attributes["src"].Value;
                // get the links of episodes and servers for each one
                Anime.EpisodeServers = GetEpisodeServers(doc);
                // add to anime obj
                animes.Add(Anime);

            }
            


        }
        return animes;
    }


    /*
      static List<string> GetepisodLinks(string url) {
            var doc = GetDocument(url);
            var Nodes = doc.DocumentNode.SelectNodes("//div[@class=\"episodes-card-title\"]/h3/a");
            var baseUri = new Uri(url);
            var links = new List<string>();
            if(Nodes != null) {
                foreach (var node in Nodes)
                {
                    var link = node.Attributes["href"].Value;
                    link = new Uri(baseUri, link).AbsoluteUri;
                    links.Add(link);
                }
            }
            return links;

        }
    */

    static Dictionary<string, List<string>> GetEpisodeServers(HtmlDocument doc)
    {
        var episodeServers = new Dictionary<string, List<string>>();

        var episodeNodes = doc.DocumentNode.SelectNodes("//div[@class=\"ep-card-anime-title-detail\"]/h3/a");

        if (episodeNodes != null)
        {
            foreach (var episodeNode in episodeNodes)
            {
                var episodeUrl = episodeNode.Attributes["href"].Value;
                var servers = new List<string>();

                var episodeDocument = GetDocument(episodeUrl);
                var serverNodes = episodeDocument.DocumentNode.SelectNodes("//ul[@id=\"episode-servers\"]/li/a[@data-ep-url]");

                if (serverNodes != null)
                {
                    foreach (var serverNode in serverNodes)
                    {
                        var serverUrl = serverNode.Attributes["data-ep-url"].Value;
                        servers.Add(serverUrl);
                    }
                }

                episodeServers.Add(episodeUrl, servers);
            }
        }

        return episodeServers;
    }



    static List<string> GetAnimeLinks(string url)
        {
            var doc = GetDocument(url);
            var animeNodes = doc.DocumentNode.SelectNodes("//div[@class=\"anime-card-title\"]/h3/a");

            var baseUri = new Uri(url);
            var links = new List<string>();
            foreach (var node in animeNodes)
            {
                var link = node.Attributes["href"].Value;
                link = new Uri(baseUri, link).AbsoluteUri;
                links.Add(link);
            }
            return links;
        }

    static HtmlDocument GetDocument(string url)
        {
            var web = new HtmlWeb(); // Create new HtmlWeb object
            HtmlDocument doc = web.Load(url); // Load this document 
            return doc;
        }
}
