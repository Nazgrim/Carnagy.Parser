using System;
using ParserEngine;
using Autofac;
using DataAccess;
using DataAccess.Repositories;
using ParserEngine.DealerParser;

namespace Parser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = BuildContainer();
            var parser = container.Resolve<IParser>();

            parser.Run();

            Console.WriteLine("Parsing is completed. Please press any key.");
            Console.ReadKey();
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<BaseRepository>().As<IBaseRepository>();
            builder.RegisterType<AutoTraderParser>().As<IParser>();
            builder.RegisterType<CarnagyContext>().AsSelf();

            return builder.Build();
        }
    }
}
