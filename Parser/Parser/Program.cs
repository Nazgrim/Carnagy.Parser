using System;
using ParserEngine;
using Autofac;
using DataAccess;
using DataAccess.Repositories;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<BaseRepository>().As<IBaseRepository>();
            builder.RegisterType<SiteParser>().As<ISiteParser>();
            builder
                .RegisterType<CarnagyContext>()
                .AsSelf();
            var container = builder.Build();

            var siteParser = container.Resolve<ISiteParser>();
            siteParser.Run();
            Console.WriteLine("Парсинг закончился. Нажмите любую клавишу для завершения.....");
            Console.ReadKey();
        }
    }
}
