using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Repositories;
using DataAccess.Models;
using HtmlAgilityPack;

namespace ParserEngine
{
    public class SiteParser : ISiteParser
    {
        private IBaseRepository _repository { get; set; }
        private List<string> _errorLog = new List<string>();
        public SiteParser(IBaseRepository repository)
        {
            _repository = repository;
        }
        public void Run()
        {
            _repository.ClearParssed();
            var mainConfigurations = _repository.GetMainConfigurations();
            foreach (var mainConfiguration in mainConfigurations)
            {
                var resultFirstPage = FirstPhase(mainConfiguration.SiteUrl, mainConfiguration.Fields.ToList());
                SecondPhase(resultFirstPage);
            }
        }

        private HtmlDocument GetHtmlDocument()
        {
            var sourceDirectory= System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            var path = System.IO.Path.Combine(sourceDirectory, "autotrader\\New & Used Cars for sale in Ontario _ autoTRADER.ca.html");
            var html = System.IO.File.ReadAllText(path);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private List<ParssedCar> FirstPhase(string url, List<Field> fields)
        {
            var htmlDocument = GetHtmlDocument();
            var result = new List<ParssedCar>();

            var carListNodes = htmlDocument.GetElementbyId("adList")
                .ChildNodes
                .Where(a => a.Name == "div")
                .ToList()
                .Where(a => a.Attributes.Contains("class") && a.Attributes["class"].Value.Contains("at_featuredResult"))
                .ToList();

            foreach (var carListNode in carListNodes)
            {
                result.Add(ParseCarNode(fields, carListNode, url));
            }
            _repository.SaveParssedCar(result);
            return result;
        }

        private ParssedCar ParseCarNode(List<Field> fields, HtmlNode carListNode,string url)
        {
            //var ImgPath = carListNode.SelectSingleNode("div[1]/div[1]/div[2]/span[1]/a[1]/img[1]").GetAttributeValue("src", string.Empty);
            //var DillerName = carListNode.SelectSingleNode("div[1]/div[2]/div[2]/div[1]/div[1]/a[1]/img[1]").GetAttributeValue("alt", string.Empty).Trim();
            //var DillerLogo = carListNode.SelectSingleNode("div[1]/div[2]/div[2]/div[1]/div[1]/a[1]/img[1]").GetAttributeValue("src", string.Empty).Trim();

            //var Price = carListNode.SelectSingleNode("div[1]/div[1]/div[2]/div[2]/div[1]/text()[2]").InnerText.Trim();
            //var lenght = carListNode.SelectSingleNode("div[1]/div[1]/div[2]/div[2]/div[2]").InnerText.Trim();
            //var Name = carListNode.SelectSingleNode("div[1]/div[2]/div[1]/h2[1]/a[1]/span[1]").InnerText.Trim();
            //var description = carListNode.SelectSingleNode("div[1]/div[2]/div[1]/p[1]").InnerText.Trim();


            //var Url = carListNode.SelectSingleNode("div[1]/div[1]/div[2]/span[1]/a[1]").GetAttributeValue("href", string.Empty).Split('?')[0];
            //var DillerPlace = carListNode.SelectSingleNode("div[1]/div[2]/div[2]/div[1]/div[4]").InnerText.Replace("&nbsp;", "").Trim();

            var parssedCar = new ParssedCar();
            foreach (var field in fields)
            {
                var filedValue = GetFieldValue(field, carListNode,url);
                if (filedValue != null)
                {
                    parssedCar.FieldValues.Add(filedValue);
                }
            }
            return parssedCar;
        }

        private FieldValue GetFieldValue(Field field, HtmlNode carListNode, string url)
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
                _errorLog.Add(string.Format("Field Id:{0}\nUrl:{1}\nErrorMessage:{2}\nInnerHtml:{3}", field.Id, url, ex.Message, carListNode.InnerHtml));
            }
            return null;
        }

        private void SecondPhase(List<ParssedCar> parrsedCars)
        {

        }
    }
}
