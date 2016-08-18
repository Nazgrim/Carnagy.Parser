using System;
using ParserEngine;
using Autofac;
using DataAccess;
using DataAccess.Repositories;
using ParserEngine.DealerParser;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<BaseRepository>().As<IBaseRepository>();
            builder.RegisterType<AutoTraderParser>().As<IParser>();
            builder
                .RegisterType<CarnagyContext>()
                .AsSelf();
            var container = builder.Build();
            var siteParser = container.Resolve<IParser>();
            siteParser.Run();
            Console.WriteLine("Парсинг закончился. Нажмите любую клавишу для завершения.....");
            Console.ReadKey();
        }
    }
}
