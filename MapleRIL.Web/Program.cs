using System;
using Nancy.Hosting.Self;
using MapleRIL.Web.Struct;
using Newtonsoft.Json;
using System.IO;
using MapleRIL.Common;

namespace MapleRIL.Web
{
    class Program
    {
        public static Config Config;

        static void Main(string[] args)
        {
            Console.WriteLine("MapleRIL Web Server");
            Console.WriteLine("-------------------------------");

            Console.WriteLine("Loading config...");
            Config = JsonConvert.DeserializeObject<Config>(
                File.ReadAllText("./config.json"));

            Console.WriteLine("Loading WZs...");
            foreach (var r in Config.Regions)
            {
                RILManager.LoadSearcher(r.Region, r.FolderPath);
                Console.WriteLine($"Loaded {r.Region} WZ + searcher. ({r.FolderPath})");
            }

            Console.WriteLine("-------------------------------");

            var uri = new Uri("http://localhost:" + Config.Port.ToString());
            using (var host = new NancyHost(uri))
            {
                host.Start();

                Console.WriteLine("Your application is running on " + uri);
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }
}
