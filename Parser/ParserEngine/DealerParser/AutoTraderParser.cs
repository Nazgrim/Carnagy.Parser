using DataAccess.Models;
using DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using HtmlAgilityPack;
using Utility;

namespace ParserEngine.DealerParser
{
    public class AutoTraderParser : BaseParser
    {
        public AutoTraderParser(IParseRepository repository) :
            base(repository, "AutoTrader")
        {
        }

        protected override string GetPageUrl(string format, Dictionary<string, object> arg)
        {
            var displayCarInPage = 100;
            var radius = "-1";
            return string.Format(
                        "{0}/?prx={1}&rcs={2}&rcp={3}&adtype=Dealer&sts=New&showcpo=1&hprc=True&wcp=False",
                        format, radius, (int)arg["page"] * displayCarInPage, displayCarInPage);
        }

        #region ForDebugOnly
        protected override List<ParsedCar> FirstPhase(string url, List<Field> fields)
        {
            var result = new List<ParsedCar>();
            var isLastPage = false;
            var page = 0;
            var pagerCurrentPageField = fields.First(a => a.Name == FiledNameConstant.PagerCurrentPage);

            var listMakers = new List<string> {"chevrolet", "buick", "GMC"};

            foreach (var maker in listMakers.Where(a=>a== "buick"))
            {
                url =
                    $"http://www.autotrader.ca/cars/{maker}";

                while (!isLastPage)
                {
                    WriteToLog("Страница " + page);
                    var pageUrl =//"http://www.autotrader.ca/cars/buick/encore/?prx=-1&loc=Ontario&adtype=Dealer&sts=New&hprc=True&wcp=True&inMarket=advancedSearch&rcs=0&rcp=500";
                        GetPageUrl(url, new Dictionary<string, object> { { "page", page } });
                    var htmlDocument = GetHtmlDocument(pageUrl);
                    if (htmlDocument == null)
                    {
                        WriteToLog("Последняя страница");
                        isLastPage = true;
                        continue;
                    }
                    result.AddRange(ParseListCars(htmlDocument, fields));
                    page++;
                    var pageNumberWrapper = htmlDocument.DocumentNode.SelectSingleNode(pagerCurrentPageField.Xpath);
                    isLastPage = pageNumberWrapper == null;
                }
            }

            UpdateDeleteParsedCar();
            SaveError();
            return result;
        }

        //protected override void SecondPhase(List<ParssedCar> parrsedCars, List<Field> fields)
        //{
        //    var htmlWeb = new HtmlWeb();
        //    foreach (var parrsedCar in parrsedCars)
        //    {
        //        var htmlDocument = GetHtmlDocument2();
        //        var fieldValues = new List<FieldValue>();
        //        foreach (var field in fields.Where(a => !a.IsDefault))
        //        {
        //            var fieldValue = GetFieldValue(field, htmlDocument.DocumentNode);
        //            if (fieldValue != null)
        //            {
        //                fieldValue.ParssedCarId = parrsedCar.Id;
        //                fieldValues.Add(fieldValue);
        //            }
        //        }
        //        Repository.AddFiledsValue(fieldValues);
        //    }
        //}

        //protected override HtmlDocument GetHtmlDocument(HtmlWeb htmlWeb, string url)
        //{
        //    var sourceDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
        //    var path = System.IO.Path.Combine(sourceDirectory, "autotrader\\New & Used Cars for sale in Ontario _ autoTRADER.ca.html");
        //    var html = System.IO.File.ReadAllText(path);
        //    var htmlDocument = new HtmlDocument();
        //    htmlDocument.LoadHtml(html);
        //    return htmlDocument;
        //}

        private HtmlDocument GetHtmlDocument2()
        {
            var sourceDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            var path = System.IO.Path.Combine(sourceDirectory, "autotrader\\1970 Chevrolet Chevelle SS for $65,000 in KITCHENER _ autoTRADER.ca.html");
            var html = System.IO.File.ReadAllText(path);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private string GetPageUrl(int page, string url, int displayCarInPage = 15, string radius = "-1", string make = "", string province = "", string location = "")
        {
            switch (radius)
            {
                case "-1":
                    return string.Format(
                        "{3}/cars/?prx={0}&rcs={1}&rcp={2}&adtype=Dealer&sts=New&showcpo=1&hprc=True&wcp=False",
                        radius, page * displayCarInPage, displayCarInPage, url);
                case "-2":
                    return string.Format(
                        "{6}/cars/{0}/on/?prx={1}&prv={2}&loc={3}&rcs={4}&rcp={5}&adtype=Dealer&sts=New&showcpo=1&hprc=True&wcp=False",
                        make, radius, province, location, page * displayCarInPage, displayCarInPage, url);
                default:
                    return string.Format(
                        "{6}/cars/{0}/on/thornhill/?prx={1}&prv={2}&loc={3}&rcs={4}&rcp={5}&adtype=Dealer&sts=New&showcpo=1&hprc=True&wcp=False",
                        make, radius, province, location, page * displayCarInPage, displayCarInPage, url);
            }
        }

        #endregion
    }
}