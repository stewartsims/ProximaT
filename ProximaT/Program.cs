using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProximaT
{
    class Program
    {
        public static int SLOP = 100;
        public static int SNIPPET_SIZE = 256;

        static void Main(string[] args)
        {
            /*
            if (args == null || args.Length == 0)
            {
                throw new ApplicationException("Specify the URI of the resource to retrieve.");
            }
            */

            string url = "https://en.wikipedia.org/wiki/List_of_Wheeler_Dealers_episodes";

            WebClient client = new WebClient();

            // Add a user agent header in case the 
            // requested URI contains a query.

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            //Stream data = client.OpenRead(args[0]);
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string page = reader.ReadToEnd();
            data.Close();
            reader.Close();

            //Console.WriteLine("Retrieved page, enter search terms:");
            //string query = Console.ReadLine();
            //string[] terms = query.Split(' ');

            Console.WriteLine("Enter marque or model (e.g. `VW' or `Beetle'):");
            string objectInput = Console.ReadLine();
            Console.WriteLine("Enter subject (e.g. `interior' or `service'):");
            string subjectInput = Console.ReadLine();
            
            if (string.IsNullOrEmpty(objectInput) && string.IsNullOrEmpty(subjectInput))
            {
                Console.WriteLine("Invalid input");
                return;
            }

            string proximityRegex = "\\b(?:" + objectInput + "(?:\\W+\\w+){1," + SLOP + "}?\\W+" + subjectInput + "|" + subjectInput + "(?:\\W+\\w+){1," + SLOP + "}?\\W+" + objectInput + ")\\b";

            if (string.IsNullOrEmpty(objectInput))
            {
                proximityRegex = subjectInput;
            } 
            if (string.IsNullOrEmpty(subjectInput))
            {
                proximityRegex = objectInput;
            }
            
            Regex regex = new Regex(proximityRegex);
            List<int> hits = new List<int>();
            foreach (Match match in regex.Matches(page))
            {
                hits.Add(match.Index);
            }

            List<string> snippets = new List<string>(hits.Count);
            hits.ForEach(hit => {
                int snippetStart = (hit - SNIPPET_SIZE/2) > 0 ? hit - SNIPPET_SIZE/2 : 0;
                string snippet = "..." + page.Substring(snippetStart, SNIPPET_SIZE) + "...";
                string pageToHit = page.Substring(0, hit);
                int seriesPosition = pageToHit.LastIndexOf(">Series ")+1;
                string seriesText = null;
                if (seriesPosition > 0)
                {
                    string textAfterSeries = pageToHit.Substring(seriesPosition);
                    if (textAfterSeries.IndexOf('<') > 0 && pageToHit.LastIndexOf("<td class=\"summary\" style=\"text-align:left\">") > 0)
                    {
                        seriesText = textAfterSeries.Substring(0, textAfterSeries.IndexOf('<'));
                        string pageToVehicle = pageToHit.Substring(0, pageToHit.LastIndexOf("<td class=\"summary\" style=\"text-align:left\">"));
                        int episodePosition = pageToVehicle.LastIndexOf("<td style=\"text-align:center\">") + 30;
                        if (episodePosition > 0)
                        {
                            string textAfterEpisode = pageToVehicle.Substring(episodePosition);
                            string episodeText = textAfterEpisode.Substring(0, textAfterEpisode.IndexOf('<'));
                            seriesText += ", Episode " + episodeText;
                        }
                        snippet = seriesText + "\n" + snippet;
                    }
                }
                if (seriesText != null && !snippets.Any(snippetToCheck => snippetToCheck.Contains(seriesText)))
                {
                    snippets.Add(snippet);
                }
            });

            Console.WriteLine(snippets.Count + " match" + (snippets.Count != 1 ? "es" : "") + " found");
            foreach (string snippet in snippets)
            {
                Console.WriteLine();
                Console.WriteLine(snippet);
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
