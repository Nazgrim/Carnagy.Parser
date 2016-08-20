using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Repositories;
using DataAccess.Models;
using HtmlAgilityPack;
using DataAccess;
using ParserEngine.Extensions;
using System.Threading;
using System.Net;

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

        public BaseParser(IBaseRepository repository, string parserName)
        {
            Repository = repository;
            ParserName = parserName;
        }

        public virtual void Run()
        {
            Repository.ClearParssed();
            LastUpdate = DateTime.Now;
            var mainConfiguration = Repository.GetMainConfigurationByName(ParserName);
            mainConfiguration.LastTimeUpdate = LastUpdate;
            Repository.SaveChanges();
            MainConfigurationId = mainConfiguration.Id;
            var fileds = mainConfiguration.Fields.Where(a => a.ConfigurationType == FiledConfigurationType.List).ToList();
            var resultFirstPage = FirstPhase(mainConfiguration.SiteUrl, fileds);
            fileds = mainConfiguration.Fields.Where(a => a.ConfigurationType == FiledConfigurationType.Page).ToList();
            SecondPhase(resultFirstPage, fileds);
        }

        protected virtual List<ParssedCar> FirstPhase(string url, List<Field> fields)
        {
            var result = new List<ParssedCar>();
            var htmlWeb = new HtmlWeb();
            var isLastPage = false;
            var page = 0;
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
                result.AddRange(ParsLisCar(htmlDocument, fields));
                page++;
                var pageNumberWrapper = htmlDocument.DocumentNode.SelectSingleNode(pagerCurrentPageField.Xpath);
                isLastPage = pageNumberWrapper == null;
            }

            UpdateDeleteParsedCar();
            SaveError();
            return result;
        }

        protected virtual string GetPageUrl(string str, Dictionary<string, object> arg)
        {
            return str.FormatFromDictionary(arg);
        }

        protected virtual void SecondPhase(List<ParssedCar> parrsedCars, List<Field> fields)
        {
            var htmlWeb = new HtmlWeb();
            foreach (var parrsedCar in parrsedCars)
            {
                var htmlDocument = GetHtmlDocument(htmlWeb, parrsedCar.Url);
                if (htmlDocument == null)
                {
                    continue;
                }
                var fieldValues = new List<FieldValue>();
                foreach (var field in fields.Where(a => !a.IsDefault))
                {
                    var fieldValue = GetFieldValue(field, htmlDocument.DocumentNode, parrsedCar.Url);
                    if (fieldValue != null)
                    {
                        fieldValue.ParssedCarId = parrsedCar.Id;
                        fieldValues.Add(fieldValue);
                    }
                }
                Repository.AddFiledsValue(fieldValues);
            }
            SaveError();
        }

        protected virtual FieldValue GetFieldValue(Field field, HtmlNode carListNode, string url = "")
        {
            try
            {
                var filedValue = new FieldValue { FieldId = field.Id };
                if (string.IsNullOrWhiteSpace(field.Attribute))
                {
                    filedValue.Value = carListNode.SelectSingleNode(field.Xpath).InnerText.Trim();
                }
                else
                {
                    filedValue.Value = carListNode.SelectSingleNode(field.Xpath).GetAttributeValue(field.Attribute, string.Empty).Trim();
                }
                return filedValue;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Empty;
                if (!string.IsNullOrWhiteSpace(url))//нельзя сохранять всю html страницу
                {
                    errorMessage = string.Format("Field Id:{0}\nErrorMessage:{1}\nUrl:{2}\nInnerExeption{3}",
                    field.Id,
                    ex.Message,
                    url,
                    ex.InnerException?.Message ?? string.Empty);
                }
                else
                {
                    errorMessage = string.Format("Field Id:{0}\nErrorMessage:{1}\nInnerHtml:{2}\nInnerExeption{3}",
                    field.Id,
                    ex.Message,
                    carListNode.InnerHtml,
                    ex.InnerException?.Message ?? string.Empty);
                }
                Log(errorMessage);
            }
            return null;
        }

        protected virtual ParssedCar ParseCarNode(IEnumerable<Field> fields, HtmlNode carListNode)
        {
            var parssedCar = new ParssedCar
            {
                MainConfigurationId = MainConfigurationId,
                CreatedTime = LastUpdate,
                LastUpdate = LastUpdate
            };
            var urlField = fields.First(a => a.Name == FiledNameConstant.Url);
            var urlFieldValue = GetFieldValue(urlField, carListNode);
            if (string.IsNullOrWhiteSpace(urlFieldValue?.Value))
                return null;

            parssedCar.Url = urlFieldValue.Value;

            var priceField = fields.First(a => a.Name == FiledNameConstant.Price);
            var priceFieldValue = GetFieldValue(priceField, carListNode);
            var price = new Price { DateTime = LastUpdate };
            if (priceFieldValue != null)
                price.Value = priceFieldValue.Value;

            parssedCar.Prices.Add(price);

            foreach (var field in fields.Where(a => !a.IsDefault))
            {
                var filedValue = GetFieldValue(field, carListNode);
                if (filedValue != null)
                {
                    parssedCar.FieldValues.Add(filedValue);
                }
            }
            return parssedCar;
        }

        protected virtual List<ParssedCar> ParsLisCar(HtmlDocument htmlDocument, List<Field> fields)
        {
            var listField = fields.First(a => a.Name == FiledNameConstant.List);
            var carListNodes = htmlDocument.DocumentNode.SelectNodes(listField.Xpath).ToList();

            var parsedCars = new List<ParssedCar>();
            foreach (var carListNode in carListNodes)
            {
                var parsedCar = ParseCarNode(fields, carListNode);
                if (parsedCar != null)
                {
                    parsedCars.Add(parsedCar);
                }
            }

            var urls = parsedCars.Select(a => a.Url).ToList();
            var parsedCarsFormDataBase =
                Repository.GetParssedCars(a => a.MainConfigurationId == MainConfigurationId && urls.Contains(a.Url));

            var newCars = new List<ParssedCar>();
            foreach (var parssedCar in parsedCars)
            {
                var parsedCarFormDataBase = parsedCarsFormDataBase.FirstOrDefault(a => a.Url == parssedCar.Url);
                if (parsedCarFormDataBase == null)
                {
                    newCars.Add(parssedCar);
                }
                else
                {
                    parsedCarFormDataBase.LastUpdate = LastUpdate;
                }
            }

            Repository.SaveParssedCar(newCars);
            return newCars;
        }

        protected virtual HtmlDocument GetHtmlDocument(HtmlWeb htmlWeb, string url)
        {
            HtmlDocument result = null;
            for (var i = 0; i < NumberOfRetries; i++)
            {
                try
                {
                    result = htmlWeb.Load(url);
                    return result;
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
            var deleteParsedCar =
                            Repository.GetParssedCars(
                                a => a.MainConfigurationId == MainConfigurationId && a.LastUpdate != LastUpdate);
            foreach (var parssedCar in deleteParsedCar)
            {
                parssedCar.IsDeleted = true;
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

