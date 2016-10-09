using System;
using System.Diagnostics;
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
            //    var stopwathc= new Stopwatch();
            //stopwathc.Start();
            //download.Download();
            //stopwathc.Stop();
            var parser = container.Resolve<IParser>();
            Console.WriteLine("Parsing is started.");
            parser.Run();
            Console.WriteLine("Parsing is completed.");

            //var parseAndAnalyze = container.Resolve<IParseAndAnalyze>();
            //Console.WriteLine("AddisongmParseAndAnalyze is started.");
            //parseAndAnalyze.Run();
            //Console.WriteLine("AddisongmParseAndAnalyze is completed.");

            //var analyzer = container.Resolve<IAnalyzer>();
            //Console.WriteLine("Analyzer is started.");
            //analyzer.Run();
            //Console.WriteLine("Analyzer is completed.");

            Console.ReadKey();
        }

        private static IContainer BuildContainer()
        {
            const string baseDir = @"C:\Users\Иван\Source\Repos\Carnagy.Parser\Parser\WepApi\Image";
            var builder = new ContainerBuilder();

            builder.RegisterType<BaseRepository>().As<IBaseRepository>();
            builder.RegisterType<AutoTraderParser>().As<IParser>();
            builder.RegisterType<CarnagyContext>().AsSelf();
            builder.RegisterType<AnalyzerBase>().As<IAnalyzer>();
            builder.RegisterType<ParseAndAnalyze>().As<IParseAndAnalyze>();
            builder.RegisterType<DownloadImage>().As<IDownloadImage>()
                  .WithParameter("baseDir", baseDir);

            return builder.Build();
        }
    }
}
