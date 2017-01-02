using System;
using System.Linq;
using AddisongmParseAndAnalyze;
using AnalyzerEngine;
using Autofac;
using DataAccess;
using DataAccess.Repositories;
using ParserEngine;
using ParserEngine.DealerParser;
using Utility;
using System.Configuration;

namespace Runner
{
    class Program
    {
        private static void Main(string[] args)
        {
            var container = BuildContainer();
            //Helper.DownloadImage(container.Resolve<CarnagyContext>(), container.Resolve<IDownloadImage>());

            var parser = container.Resolve<IParser>();
            Console.WriteLine("Parsing is started.");
            parser.Run();
            Console.WriteLine("Parsing is completed.");

            var analyzer = container.Resolve<IAnalyzer>();
            Console.WriteLine("Analyzer is started.");
            analyzer.Run();
            Console.WriteLine("Analyzer is completed.");

            var parseAndAnalyze = container.Resolve<IParseAndAnalyze>();
            Console.WriteLine("AddisongmParseAndAnalyze is started.");
            parseAndAnalyze.Run();
            Console.WriteLine("AddisongmParseAndAnalyze is completed.");

            Console.WriteLine("Сalculation is started.");
            analyzer.Сalculation();
            Console.WriteLine("Сalculation is completed.");
        }

        private static IContainer BuildContainer()
        {
            var threadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCount"]);
            var baseDir = ConfigurationManager.AppSettings["CarImageFolder"];

            var builder = new ContainerBuilder();

            builder.RegisterType<ParseRepository>().As<IParseRepository>();
            builder.RegisterType<AnalyseRepository>().As<IAnalyseRepository>();
            builder.RegisterType<AutoTraderParser>().As<IParser>();
            builder.RegisterType<CarnagyContext>().AsSelf();
            builder.RegisterType<AnalyzerBase>().As<IAnalyzer>();
            builder.RegisterType<ParseAndAnalyze>().As<IParseAndAnalyze>();
            builder.RegisterType<DownloadManager>().As<IDownloadManager>();
            builder.RegisterType<ImageDownloader>().As<IDownloadImage>()
                  .WithParameter(nameof(baseDir), baseDir)
                  .WithParameter(nameof(threadCount), threadCount);

            return builder.Build();
        }
    }
}
