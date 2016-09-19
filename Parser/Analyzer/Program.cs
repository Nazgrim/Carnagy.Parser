using System;
using AnalyzerEngine;
using Autofac;
using DataAccess;
using DataAccess.Repositories;

namespace Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<BaseRepository>().As<IBaseRepository>();
            builder.RegisterType<AnalyzerBase>().As<IAnalyzer>();
            builder
                .RegisterType<CarnagyContext>()
                .AsSelf();
            var container = builder.Build();
            var analyzer = container.Resolve<IAnalyzer>();
            analyzer.Run();
            Console.WriteLine("Анализ закончился. Нажмите любую клавишу для завершения.....");
            Console.ReadKey();
        }
    }
}
