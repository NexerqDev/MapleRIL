using MapleRIL.Web.Struct;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.Json;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web.Hosting;

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

            // are we in asp or are we standalone?
            bool isAsp = HostingEnvironment.IsHosted;
            string basePath = isAsp ? System.Web.HttpRuntime.BinDirectory : AppDomain.CurrentDomain.BaseDirectory;

            Console.WriteLine("Loading config...");
            Config = JsonConvert.DeserializeObject<Config>(
                File.ReadAllText(Path.Combine(basePath, "config.json")));

            Console.WriteLine("Loading WZs...");
            foreach (var r in Config.Regions)
            {
                RILManager.LoadJson(r.Region, Path.Combine(basePath, r.JsonPath));
                Console.WriteLine($"Loaded {r.Region} JSON data. ({r.JsonPath})");
            }

            Console.WriteLine("-------------------------------");

            // no limit bois
            JsonSettings.MaxJsonLength = Int32.MaxValue;
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