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
                var resultFirstPage = FirstPhase(mainConfiguration.Fields.ToList());
                SecondPhase(resultFirstPage);
            }
        }

        private HtmlDocument GetHtmlDocument()
        {
            var path = @"C:\Users\Иван\Desktop\autotrader\New & Used Cars for sale in Ontario _ autoTRADER.ca.html";
            var html = System.IO.File.ReadAllText(path);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private List<ParssedCar> FirstPhase(List<Field> fields)
        {
            var htmlDocument = GetHtmlDocument();
            var result = new List<ParssedCar>();

            var carListNodes = htmlDocument.GetElementbyId("adList")
                .ChildNodes
                .Where(a => a.Name == "div")
                .ToList()
                .Where(a=>a.Attributes.Contains("class")&&a.Attributes["class"].Value.Contains("at_featuredResult"))
                .ToList();
            //.DocumentNode.SelectNodes("//*[@id='resultsLeftCol']/div[5]");
            foreach (var carListNode in carListNodes)
            {
                result.Add(ParseCarNode(fields, carListNode));
                //if (carListNode.Attributes.Contains("class") &&
                //           carListNode.Attributes["class"].Value.Contains("at_priorityResult"))
                //{

                //}
                //else
                //{
                    
                //}

            }
            _repository.SaveParssedCar(result);
            return result;
        }

        private ParssedCar ParseCarNode(List<Field> fields, HtmlNode carListNode)
        {
            var parssedCar = new ParssedCar();
            var abc = carListNode.SelectSingleNode("div[1]/div[1]/div[2]/span[1]/a[1]");
            //var ImgPath = img.GetAttributeValue("src", string.Empty);
            //var Price = price.InnerText.Trim();
            //var Url = divBodyLink.GetAttributeValue("href", string.Empty).Split('?')[0];
            //var Name = h2Text.InnerText.Trim();
            //var DillerName = dillerImage.GetAttributeValue("alt", string.Empty).Trim();
            //var DillerLogo = dillerImage.GetAttributeValue("src", string.Empty).Trim();
            //var DillerPlace = dillerImageDiv.ChildNodes[5].InnerText.Replace("&nbsp;", "").Trim();

            foreach (var field in fields)
            {
                var filedValue = new FieldValue();
                filedValue.FieldId = field.Id;
                parssedCar.FieldValues.Add(filedValue);
            }
            return parssedCar;
        }

        private void SecondPhase(List<ParssedCar> parrsedCars)
        {

        }
    }
}
