using System;
using AddisongmParseAndAnalyze;
using AnalyzerEngine;
using Autofac;
using DataAccess;
using DataAccess.Repositories;
using ParserEngine;
using ParserEngine.DealerParser;
using Utility;

namespace Runner
{
    class Program
    {
        private static void Main(string[] args)
        {
            var container = BuildContainer();

            //var download = container.Resolve<IDownloadImage>();
            //var stopwathc= new Stopwatch();
            //stopwathc.Start();
            //download.Download();
            //stopwathc.Stop();

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
        }

        private static IContainer BuildContainer()
        {
            //TODO: Move to app.config
            const int threadCount = 5;
            const string baseDir = @"C:\Users\Иван\Source\Repos\Carnagy.Parser\Parser\WepApi\Image";

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
