using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Repositories;
using DataAccess.Models;
using HtmlAgilityPack;
using DataAccess;
using ParserEngine.Extensions;

namespace ParserEngine
{
    public class BaseParser : IParser
    {
        protected readonly string ParserName;
        protected int MainConfigurationId;
        protected IBaseRepository _repository { get; set; }
        protected List<string> _errorLog = new List<string>();
        protected DateTime LastUpdate;

        protected BaseParser(IBaseRepository repository)
        {
            _repository = repository;
        }

        public BaseParser(IBaseRepository repository, string parserName)
        {
            _repository = repository;
            ParserName = parserName;
        }

        public virtual void Run()
        {
            _repository.ClearParssed();
            LastUpdate= DateTime.Now;
            var mainConfiguration = _repository.GetMainConfigurationByName(ParserName);
            MainConfigurationId = mainConfiguration.Id;
            var fileds = mainConfiguration.Fields.Where(a => a.ConfigurationType == FiledConfigurationType.List).ToList();
            var resultFirstPage = FirstPhase(mainConfiguration.SiteUrl, fileds);
            fileds = mainConfiguration.Fields.Where(a => a.ConfigurationType == FiledConfigurationType.Page).ToList();
            SecondPhase(resultFirstPage, fileds);
        }

        protected List<ParssedCar> FirstPhase(string url, List<Field> fields)
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
                result.AddRange(ParsLisCar(htmlDocument, fields));
                page++;
                var pageNumberWrapper = htmlDocument.DocumentNode.SelectSingleNode(pagerCurrentPageField.Xpath);
                isLastPage = pageNumberWrapper != null;
            }

            UpdateDeleteParsedCar();

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
                var fieldValues = new List<FieldValue>();
                foreach (var field in fields.Where(a => !a.IsDefault))
                {
                    var fieldValue = GetFieldValue(field, htmlDocument.DocumentNode);
                    if (fieldValue != null)
                    {
                        fieldValue.ParssedCarId = parrsedCar.Id;
                        fieldValues.Add(fieldValue);
                    }
                }
                _repository.AddFiledsValue(fieldValues);
            }
        }

        protected virtual FieldValue GetFieldValue(Field field, HtmlNode carListNode)
        {
            try
            {
                var filedValue = new FieldValue();
                filedValue.FieldId = field.Id;
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
                _errorLog.Add(string.Format("Field Id:{0}\nErrorMessage:{1}\nInnerHtml:{2}", field.Id, ex.Message, carListNode.InnerHtml));
            }
            return null;
        }

        protected virtual ParssedCar ParseCarNode(IEnumerable<Field> fields, HtmlNode carListNode)
        {
            var parssedCar = new ParssedCar
            {
                MainConfigurationId = MainConfigurationId
            };
            var urlField = fields.First(a => a.Name == FiledNameConstant.Url);
            var urlFieldValue = GetFieldValue(urlField, carListNode);
            if (urlFieldValue == null)
                return null;

            parssedCar.Url = urlFieldValue.Value;
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
            var parsedCars = new List<ParssedCar>();

            var listField = fields.First(a => a.Name == FiledNameConstant.List);
            var carListNodes = htmlDocument.DocumentNode.SelectNodes(listField.Xpath).ToList();

            foreach (var carListNode in carListNodes)
            {
                parsedCars.Add(ParseCarNode(fields, carListNode));
            }
            var urls = parsedCars.Select(a => a.Url).ToList();
            var parsedCarsFormDataBase =
                _repository.GetParssedCars(a => a.MainConfigurationId == MainConfigurationId && urls.Contains(a.Url));

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

            _repository.SaveParssedCar(newCars);
            return newCars;
        }

        protected virtual HtmlDocument GetHtmlDocument(HtmlWeb htmlWeb, string url)
        {
            var htmlDocument = htmlWeb.Load(url);
            return htmlDocument;
        }

        protected virtual void UpdateDeleteParsedCar()
        {
            var deleteParsedCar =
                            _repository.GetParssedCars(
                                a => a.MainConfigurationId == MainConfigurationId && a.LastUpdate != LastUpdate);
            foreach (var parssedCar in deleteParsedCar)
            {
                parssedCar.IsDeleted = true;
            }
            _repository.SaveChanges();
        }
    }
}

