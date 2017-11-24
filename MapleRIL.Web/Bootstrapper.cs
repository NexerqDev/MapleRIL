using MapleRIL.Web.Struct;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MapleRIL.Web
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        public static Config Config;
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        public Bootstrapper() : base()
        {
            Console.WriteLine("MapleRIL Web Server");
            Console.WriteLine("-------------------------------");

            Console.WriteLine("Loading config...");
            Config = JsonConvert.DeserializeObject<Config>(
                File.ReadAllText(Path.Combine(System.Web.HttpRuntime.BinDirectory, "config.json")));

            Console.WriteLine("Loading WZs...");
            foreach (var r in Config.Regions)
            {
                RILManager.LoadSearcher(r.Region, r.FolderPath);
                Console.WriteLine($"Loaded {r.Region} WZ + searcher. ({r.FolderPath})");
            }

            Console.WriteLine("-------------------------------");
        }

        // serve statics from root
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Clear();
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/Static", "Static"));
            base.ConfigureConventions(nancyConventions);
        }

        // diags page
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                if (string.IsNullOrEmpty(Config.NancyAdminPassword))
                    return new DiagnosticsConfiguration();

                return new DiagnosticsConfiguration
                {
                    Password = Config.NancyAdminPassword
                };
            }
        }


        protected override IRootPathProvider RootPathProvider => new CustomRootPathProvider();

        public class CustomRootPathProvider : IRootPathProvider
        {
            public string GetRootPath()
            {
                return System.Web.HttpRuntime.BinDirectory;
            }
        }
    }
}