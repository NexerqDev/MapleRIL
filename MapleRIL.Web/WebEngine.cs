﻿using MapleRIL.Common;
using MapleRIL.Web.Struct;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace MapleRIL.Web
{
    public static class WebEngine
    {
        public static Config Config;
        public static RILJsonManager Rjm = new RILJsonManager();

        public static bool IsAsp;
        public static string BasePath => IsAsp ? System.Web.HttpRuntime.BinDirectory : AppDomain.CurrentDomain.BaseDirectory;

        public static void Init()
        {
            Console.WriteLine("MapleRIL Web Server");
            Console.WriteLine("-------------------------------");

            // are we in asp or are we standalone?
            IsAsp = HostingEnvironment.IsHosted;

            Console.WriteLine("Loading config...");
            Config = JsonConvert.DeserializeObject<Config>(
                File.ReadAllText(Path.Combine(BasePath, "config.json")));

            Console.WriteLine("Loading WZs...");
            foreach (var r in Config.Regions)
            {
                Rjm.LoadJsonEngine(r.Region, Path.Combine(BasePath, r.JsonPath));
                Console.WriteLine($"Loaded {r.Region} JSON data. ({r.JsonPath})");
            }

            Console.WriteLine("-------------------------------");

            // no limit bois
            JsonSettings.MaxJsonLength = Int32.MaxValue;
        }
    }
}