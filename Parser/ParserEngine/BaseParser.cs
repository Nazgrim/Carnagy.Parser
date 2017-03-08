using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataAccess.Repositories;
using DataAccess.Models;
using HtmlAgilityPack;
using DataAccess;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility;
using Utility.Extensions;

namespace ParserEngine
{
    public class BaseParser : IParser
    {
        private Stopwatch _stopwatch;
        private readonly int _carListNodesThredCount = -1;
        private readonly int _fieldsThredCount = -1;
        private readonly int _compareParsedThredCount = -1;
        private readonly int _parseOnePageFieldsThredCount = -1;
        private readonly int _chunkThredCount = 10;

        private readonly int _splitByCount = 1000;

        protected readonly string ParserName;
        protected int MainConfigurationId;
        protected IParseRepository Repository { get; set; }
        protected readonly List<ErrorLog> ErrorLog = new List<ErrorLog>();
        protected DateTime LastUpdate;
        protected const int NumberOfRetries = 3;
        protected const int DelayOnRetry = 1000;

        public BaseParser(IParseRepository repository, string parserName)
        {
            Repository = repository;
            ParserName = parserName;
        }

        public virtual void Run()
        {
            LastUpdate = DateTime.Now;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var mainConfiguration = Repository.GetMainConfigurationByName(ParserName);
            mainConfiguration.LastTimeUpdate = LastUpdate;
            Repository.SaveChanges();

            MainConfigurationId = mainConfiguration.Id;

            var fields = mainConfiguration.Fields.Where(a => a.ConfigurationType == FieldConfigurationType.List).ToList();
            var resultFirstPage = FirstPhase(mainConfiguration.SiteUrl, fields);

            fields = mainConfiguration.Fields.Where(a => a.ConfigurationType == FieldConfigurationType.Page).ToList();
            Console.WriteLine($"First phase completed for {GetTime(_stopwatch.Elapsed)}");
            Console.WriteLine($"Car count is {resultFirstPage.Count}");
            SecondPhase(resultFirstPage, fields);
            _stopwatch.Stop();
            Console.WriteLine($"Second phase completed for {GetTime(_stopwatch.Elapsed)}");
        }

        private string GetTime(TimeSpan ts)
        {
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            return elapsedTime;
        }

        protected virtual void WriteToLog(string value)
        {
            Console.WriteLine("Time:{0} , Value:{1}", _stopwatch.Elapsed, value);
        }

        protected virtual List<ParsedCar> FirstPhase(string url, List<Field> fields)
        {
            var parsedCars = new List<ParsedCar>();
            var isLastPage = false;
            var page = 1;
            var pagerCurrentPageField = fields.First(a => a.Name == FiledNameConstant.PagerCurrentPage);
            while (!isLastPage)
            {
                var pageUrl = GetPageUrl(url, new Dictionary<string, object> { { "page", page } });
                var htmlDocument = GetHtmlDocument(pageUrl);
                if (htmlDocument == null)
                {
                    isLastPage = true;
                    continue;
                }
                parsedCars.AddRange(ParseListCars(htmlDocument, fields));
                page++;
                var pageNumberWrapper = htmlDocument.DocumentNode.SelectSingleNode(pagerCurrentPageField.Xpath);
                isLastPage = pageNumberWrapper == null;
            }

            UpdateDeleteParsedCar();
            SaveError();
            Repository.SaveChanges();
            return parsedCars;
        }

        protected virtual string GetPageUrl(string str, Dictionary<string, object> arg)
        {
            return str.FormatFromDictionary(arg);
        }

        protected virtual HtmlDocument GetHtmlDocument(string url)
        {
            var htmlWeb = new HtmlWeb();
            for (var i = 0; i < NumberOfRetries; i++)
            {
                try
                {
                    return htmlWeb.Load(url);
                }
                catch (WebException ex)
                {
                    var errorMessage = string.Format("Status:{0}.Message:{1}:InnerExeption{2}\n",
                        ex.Status,
                        ex.Message,
                        ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                    //WriteToLog(errorMessage);
                    if (i == NumberOfRetries)
                        return null;
                }
                catch (Exception ex)
                {
                    var errorMessage = string.Format("Message:{0}:InnerExeption{1}\n", ex.Message,
                        ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                    //WriteToLog(errorMessage);
                }
                Thread.Sleep(DelayOnRetry);
            }
            return null;
        }

        protected virtual List<ParsedCar> ParseListCars(HtmlDocument htmlDocument, List<Field> fields)
        {
            var listField = fields.First(a => a.Name == FiledNameConstant.List);
            var carListNodes = htmlDocument.DocumentNode.SelectNodes(listField.Xpath).ToList();
            WriteToLog($"Машин на странице:{carListNodes.Count}");

            var parsedCars = new ConcurrentBag<ParsedCar>();
            Parallel.ForEach(carListNodes, new ParallelOptions { MaxDegreeOfParallelism = _carListNodesThredCount }, node =>
             {
                 var parsedCar = ParseCarNode(fields, node);
                 if (parsedCar != null)
                 {
                     parsedCars.Add(parsedCar);
                 }
             });

            WriteToLog($"Удалось распарсить:{parsedCars.Count}");

            //TODO: создать функцию хеширования
            //TODO: autotraderдобавляет случайную букву в www пример wwwa
            var urls = parsedCars.Select(a => a.ForCompare).ToList();
            var savedParsedCars = Repository
                .GetParsedCars(a => a.MainConfigurationId == MainConfigurationId && urls.Contains(a.ForCompare));

            var newParsedCars = new ConcurrentBag<ParsedCar>();
            var prices = new ConcurrentBag<Price>();
            Parallel.ForEach(parsedCars, new ParallelOptions { MaxDegreeOfParallelism = _compareParsedThredCount }, parsedCar =>
            {
                var savedParsedCar = savedParsedCars.FirstOrDefault(a => a.ForCompare == parsedCar.ForCompare);// потенциальный дубляж
                if (savedParsedCar == null)
                {
                    newParsedCars.Add(parsedCar);
                }
                else
                {

                    savedParsedCar.LastUpdate = LastUpdate;
                    var currentPrice = parsedCar.Prices.FirstOrDefault() ??
                                        savedParsedCar.Prices.Last();
                    var price = new Price
                    {
                        ParsedCarId = savedParsedCar.Id,
                        Value = currentPrice.Value,
                        DateTime = LastUpdate
                    };
                    prices.Add(price);
                }
            });

            Repository.AddPrices(prices);
            var result = newParsedCars.ToList();
            Repository.SaveParsedCar(result);

            var list = new List<FieldValue>();
            var msrpFiled = fields.First(a => a.Name == FiledNameConstant.MSRP);
            foreach (var savedParsedCar in savedParsedCars)
            {
                var parsedCar = parsedCars.FirstOrDefault(a => a.ForCompare == savedParsedCar.ForCompare);
                var msrp = savedParsedCar.FieldValues.FirstOrDefault(a => a.Field.Name == FiledNameConstant.MSRP);
                if (parsedCar != null && msrp == null)
                {
                    var field = parsedCar.FieldValues.FirstOrDefault(a => a.Field.Name == FiledNameConstant.MSRP);
                    if (field == null)
                    {
                        list.Add(new FieldValue
                        {
                            Field = msrpFiled,
                            ParsedCarId = savedParsedCar.Id,
                            Value = prices.FirstOrDefault(a => a.ParsedCarId == savedParsedCar.Id).Value
                        });
                    }
                    else
                    {
                        list.Add(new FieldValue
                        {
                            Field = msrpFiled,
                            ParsedCarId = savedParsedCar.Id,
                            Value = field.Value
                        });
                    }
                }
            }
            if (list.Any())
            {
                Repository.AddFieldValues(list);
            }

            return result;
        }

        protected virtual ParsedCar ParseCarNode(IEnumerable<Field> fields, HtmlNode carListNode)
        {
            var urlField = fields.First(a => a.Name == FiledNameConstant.Url);
            var urlFieldValue = GetFieldValue(urlField, carListNode);
            if (string.IsNullOrWhiteSpace(urlFieldValue?.Value))
            {
                WriteToLog("Не удалось распарсить url");
                return null;
            }

            var parsedCar = new ParsedCar
            {
                MainConfigurationId = MainConfigurationId,
                CreatedTime = LastUpdate,
                LastUpdate = LastUpdate,
                Status = ParsedCarStatus.List,
                Url = urlFieldValue.Value.Split('?')[0],

            };

            //TODO: парсим url на предмет уникальности ключа, переделать
            parsedCar.ForCompare = parsedCar.Url.Split('/')[8].Trim();

            var priceField = fields.First(a => a.Name == FiledNameConstant.Price);
            var priceFieldValue = GetFieldValue(priceField, carListNode);
            var price = new Price { DateTime = LastUpdate };
            if (priceFieldValue != null)
            {
                var regex = new Regex(priceField.RegExPattern);
                if (regex.IsMatch(priceFieldValue.Value))
                {
                    price.Value = regex.Match(priceFieldValue.Value).Value;
                }
                else
                {
                    price.Value = priceFieldValue.Value;
                }
            }

            parsedCar.Prices.Add(price);

            var msrpFiled = fields.First(a => a.Name == FiledNameConstant.MSRP);
            var msrpFiledValue = GetFieldValue(msrpFiled, carListNode);
            if (msrpFiledValue != null)
            {
                var regex = new Regex(msrpFiled.RegExPattern);
                if (regex.IsMatch(msrpFiledValue.Value))
                {
                    msrpFiledValue.Value = regex.Match(msrpFiledValue.Value).Value;
                }
                else
                {
                    msrpFiledValue.Value = msrpFiledValue.Value;
                }
            }
            else
            {
                msrpFiledValue = new FieldValue
                {
                    Field = msrpFiled,
                    Value = price.Value
                };
            }

            var filedsValue = new ConcurrentBag<FieldValue> { msrpFiledValue };
            Parallel.ForEach(fields.Where(a => !a.IsDefault), new ParallelOptions { MaxDegreeOfParallelism = _fieldsThredCount }, field =>
            {
                var filedValue = GetFieldValue(field, carListNode);
                if (filedValue != null)
                {
                    filedsValue.Add(filedValue);
                }
                else
                {
                    //WriteToLog($"Не удалось распарсить свойство Id:{field.Id}");
                }
            });
            parsedCar.FieldValues = filedsValue.ToList();
            return parsedCar;
        }

        protected virtual FieldValue GetFieldValue(Field field, HtmlNode carListNode, string url = "")
        {
            var filedValue = new FieldValue { Field = field };
            var xpaths = field.Xpath.Split(',');
            var listError = new List<string>();
            foreach (var xpath in xpaths)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(field.Attribute))
                    {
                        filedValue.Value = carListNode.SelectSingleNode(xpath).InnerText.Trim();
                    }
                    else
                    {
                        filedValue.Value = carListNode.SelectSingleNode(xpath).GetAttributeValue(field.Attribute, string.Empty).Trim();
                    }
                    if (!string.IsNullOrEmpty(filedValue.Value))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    listError.Add($"Xpath:{xpath}-InnerExeption:{ex.InnerException?.Message ?? string.Empty}");
                }
            }

            if (!string.IsNullOrEmpty(filedValue.Value)) return filedValue;
            var urlStr = !string.IsNullOrWhiteSpace(url) ? $"Url:{url}\n" : string.Empty;
            var errorMessage = "Errors: " + string.Join("\n", listError.ToArray());
            var logMessage = $"Field Id:{field.Id}\nInnerHtml:{carListNode.InnerHtml}\n{urlStr}InnerHtml:{carListNode.InnerHtml}\n{errorMessage}";
            //WriteToLog(logMessage);
            return null;
        }

        protected virtual void UpdateDeleteParsedCar()
        {
            var deleteParsedCar = Repository
                .GetParsedCars(a => a.MainConfigurationId == MainConfigurationId && a.LastUpdate != LastUpdate);

            foreach (var parsedCar in deleteParsedCar)
            {
                parsedCar.IsDeleted = true;
            }
        }

        protected virtual void SecondPhase(List<ParsedCar> parrsedCars, List<Field> fields)
        {
            try
            {
                var chunkCollection = parrsedCars.SplitBy(_splitByCount);

                foreach (var chunk in chunkCollection)
                {
                    var collectionCarFields = new ConcurrentBag<ConcurrentBag<FieldValue>>();
                    Parallel.ForEach(chunk, new ParallelOptions { MaxDegreeOfParallelism = _chunkThredCount }, parrsedCar =>
                    {
                        var carFields = ParseOnePage(parrsedCar, fields);

                        if (!carFields.Any()) return;
                        parrsedCar.Status = ParsedCarStatus.Page;
                        collectionCarFields.Add(carFields);
                    });
                    foreach (var carFields in collectionCarFields)
                    {
                        Repository.AddFieldValues(carFields.ToList());
                    }
                    WriteToLog($"Second phase completed:{collectionCarFields.Count}");
                }
                Repository.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Down" + e.Message);
            }
        }

        protected virtual ConcurrentBag<FieldValue> ParseOnePage(ParsedCar parrsedCar, List<Field> fields)
        {
            try
            {
                var htmlDocument = GetHtmlDocument(parrsedCar.Url);
                if (htmlDocument == null)
                {
                    WriteToLog($"Не удалось загрузить {parrsedCar.Url}");
                    parrsedCar.Status = ParsedCarStatus.LoadPageError;
                    return null;
                }
                var carFields = new ConcurrentBag<FieldValue>();
                Parallel.ForEach(fields, new ParallelOptions { MaxDegreeOfParallelism = _parseOnePageFieldsThredCount }, field =>
                {
                    var fieldValue = GetFieldValue(field, htmlDocument.DocumentNode, parrsedCar.Url);
                    if (fieldValue == null) return;
                    fieldValue.ParsedCar = parrsedCar;
                    carFields.Add(fieldValue);
                });
                return carFields;
            }
            catch (Exception e)
            {
                Console.WriteLine("Up" + e.Message);
                return null;
            }
        }

        protected virtual void Log(string errorMessage)
        {
            ErrorLog.Add(new ErrorLog
            {
                MainConfigurationId = MainConfigurationId,
                DateTime = LastUpdate,
                Message = errorMessage
            });
        }

        protected virtual void SaveError()
        {
            Repository.AddErrorLog(ErrorLog);
            ErrorLog.Clear();
        }
    }
}

