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
using Utility.Extensions;

namespace ParserEngine
{
    public class BaseParser : IParser
    {
        protected readonly string ParserName;
        protected int MainConfigurationId;
        protected IBaseRepository Repository { get; set; }
        protected readonly List<ErrorLog> ErrorLog = new List<ErrorLog>();
        protected DateTime LastUpdate;
        protected const int NumberOfRetries = 3;
        protected const int DelayOnRetry = 1000;
        private Stopwatch Stopwatch { get; set; }
        protected int ThredCound = 5;

        public BaseParser(IBaseRepository repository, string parserName)
        {
            Repository = repository;
            ParserName = parserName;
        }

        public virtual void Run()
        {
            //Repository.ClearParsed();
            LastUpdate = DateTime.Now;
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            var mainConfiguration = Repository.GetMainConfigurationByName(ParserName);
            mainConfiguration.LastTimeUpdate = LastUpdate;
            Repository.SaveChanges();

            MainConfigurationId = mainConfiguration.Id;

            var fields = mainConfiguration.Fields.Where(a => a.ConfigurationType == FieldConfigurationType.List).ToList();
            var resultFirstPage = FirstPhase(mainConfiguration.SiteUrl, fields);

            fields = mainConfiguration.Fields.Where(a => a.ConfigurationType == FieldConfigurationType.Page).ToList();
            Console.WriteLine($"First phase completed for {GetTime(Stopwatch.Elapsed)}");
            Console.WriteLine($"Car count is {resultFirstPage.Count}");
            SecondPhase(resultFirstPage, fields);
            Stopwatch.Stop();
            Console.WriteLine($"Second phase completed for {GetTime(Stopwatch.Elapsed)}");
        }

        private string GetTime(TimeSpan ts)
        {
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            return elapsedTime;
        }

        protected virtual void WriteToLog(string value)
        {
            Console.WriteLine("Time:{0} , Value:{1}", Stopwatch.Elapsed, value);
        }

        protected virtual List<ParsedCar> FirstPhase(string url, List<Field> fields)
        {
            var parsedCars = new List<ParsedCar>();
            var htmlWeb = new HtmlWeb();
            var isLastPage = false;
            var page = 1;
            var pagerCurrentPageField = fields.First(a => a.Name == FiledNameConstant.PagerCurrentPage);
            while (!isLastPage)
            {
                var pageUrl = GetPageUrl(url, new Dictionary<string, object> { { "page", page } });
                var htmlDocument = GetHtmlDocument(htmlWeb, pageUrl);
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
            return parsedCars;
        }

        protected virtual string GetPageUrl(string str, Dictionary<string, object> arg)
        {
            return str.FormatFromDictionary(arg);
        }

        protected virtual void SecondPhase(List<ParsedCar> parrsedCars, List<Field> fields)
        {
            var listTask = new List<Task>();

            try
            {
                foreach (var parrsedCar in parrsedCars)
                {
                    listTask.Add(ParseOnePage(parrsedCar, fields));
                    if (listTask.Count != ThredCound)
                        continue;
                    Task.WaitAll(listTask.ToArray());
                    listTask.Clear();
                    Repository.SaveChanges();
                }
                Task.WaitAll(listTask.ToArray());
                Repository.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Down" + e.Message);
            }      
        }

        protected virtual async Task ParseOnePage(ParsedCar parrsedCar, List<Field> fields)
        {
            try
            {
                var htmlDocument = await GetHtmlDocumentAsync(parrsedCar.Url);
                if (htmlDocument == null)
                {
                    WriteToLog($"Не удалось загрузить {parrsedCar.Url}");
                    parrsedCar.Status = ParsedCarStatus.LoadPageError;
                    return;
                }
                var carFiels = (await Task.WhenAll(fields.Select(field => GetFieldAsync(field, htmlDocument, parrsedCar))))
                                    .Where(a => a != null);
                if (carFiels.Any())
                {
                    parrsedCar.Status = ParsedCarStatus.Page;
                    Repository.AddFieldValues(carFiels.ToList());
                }
            }
            catch (Exception e)
            {                
                Console.WriteLine("Up"+e.Message);
            }           
        }

        protected virtual Task<FieldValue> GetFieldAsync(Field field, HtmlDocument htmlDocument, ParsedCar parrsedCar)
        {
            return Task.Run(() =>
            {
                var fieldValue = GetFieldValue(field, htmlDocument.DocumentNode, parrsedCar.Url);
                if (fieldValue != null)
                {
                    fieldValue.ParsedCar = parrsedCar;
                    return fieldValue;
                }
                //WriteToLog($"Не удалось распарсить свойство Id:{field.Id}");
                return null;
            });
        }

        protected virtual FieldValue GetFieldValue(Field field, HtmlNode carListNode, string url = "")
        {
            var filedValue = new FieldValue { Field = field };
            var xpaths = field.Xpath.Split(',');
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
                    var errorMessage = string.Empty;
                    if (!string.IsNullOrWhiteSpace(url))//нельзя сохранять всю html страницу
                    {
                        errorMessage =
                            $"Field Id:{field.Id}\nErrorMessage:{ex.Message}\nUrl:{url}\nInnerExeption{ex.InnerException?.Message ?? string.Empty}";
                    }
                    else
                    {
                        errorMessage =
                            $"Field Id:{field.Id}\nErrorMessage:{ex.Message}\nInnerHtml:{carListNode.InnerHtml}\nInnerExeption{ex.InnerException?.Message ?? string.Empty}";
                    }
                    Log(errorMessage);
                }
            }

            return !string.IsNullOrEmpty(filedValue.Value) ? filedValue : null;
        }

        protected virtual ParsedCar ParseCarNode(IEnumerable<Field> fields, HtmlNode carListNode)
        {
            var parsedCar = new ParsedCar
            {
                MainConfigurationId = MainConfigurationId,
                CreatedTime = LastUpdate,
                LastUpdate = LastUpdate,
                Status = ParsedCarStatus.List
            };
            var urlField = fields.First(a => a.Name == FiledNameConstant.Url);
            var urlFieldValue = GetFieldValue(urlField, carListNode);
            if (string.IsNullOrWhiteSpace(urlFieldValue?.Value))
            {
                WriteToLog("Не удалось распарсить url");
                return null;
            }

            parsedCar.Url = urlFieldValue.Value;

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

            foreach (var field in fields.Where(a => !a.IsDefault))
            {
                var filedValue = GetFieldValue(field, carListNode);
                if (filedValue != null)
                {
                    parsedCar.FieldValues.Add(filedValue);
                }
                else
                {
                    //WriteToLog($"Не удалось распарсить свойство Id:{field.Id}");
                }
            }

            return parsedCar;
        }

        protected virtual List<ParsedCar> ParseListCars(HtmlDocument htmlDocument, List<Field> fields)
        {
            var listField = fields.First(a => a.Name == FiledNameConstant.List);
            var carListNodes = htmlDocument.DocumentNode.SelectNodes(listField.Xpath).ToList();
            WriteToLog($"Машин на странице:{carListNodes.Count}");

            var parsedCars = carListNodes
                .Select(node => ParseCarNode(fields, node))
                .Where(parsedCar => parsedCar != null)
                .ToList();

            WriteToLog($"Удалось распарсить:{parsedCars.Count}");

            //TODO: создать функцию хеширования
            var urls = parsedCars.Select(a => a.Url).ToList();
            var savedParsedCars = Repository
                .GetParsedCars(a => a.MainConfigurationId == MainConfigurationId && urls.Contains(a.Url));

            var newParsedCars = new List<ParsedCar>();

            foreach (var parsedCar in parsedCars)
            {
                var savedParsedCar = savedParsedCars.FirstOrDefault(a => a.Url == parsedCar.Url);// потенциальный дубляж
                if (savedParsedCar == null)
                {
                    newParsedCars.Add(parsedCar);
                }
                else
                {
                    savedParsedCar.LastUpdate = LastUpdate;
                }
            }

            Repository.SaveParsedCar(newParsedCars);

            return newParsedCars;
        }

        protected async Task<HtmlDocument> GetHtmlDocumentAsync(string url)
        {
            var request = WebRequest.Create(url);
            var response = (HttpWebResponse)await request.GetResponseAsync();
            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(response.GetResponseStream());
            return htmlDoc;
        }

        protected virtual HtmlDocument GetHtmlDocument(HtmlWeb htmlWeb, string url)
        {
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
                    Log(errorMessage);
                    if (i == NumberOfRetries)
                        return null;
                }
                catch (Exception ex)
                {
                    var errorMessage = string.Format("Message:{0}:InnerExeption{1}\n", ex.Message,
                        ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                    Log(errorMessage);
                }
                Thread.Sleep(DelayOnRetry);
            }
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

            Repository.SaveChanges();
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

