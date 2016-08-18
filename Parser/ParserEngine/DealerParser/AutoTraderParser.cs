using DataAccess.Models;
using DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ParserEngine.DealerParser
{
    public class AutoTraderParser : BaseParser, IParser
    {
        private const string ParserName = "AutoTrader";

        public AutoTraderParser(IBaseRepository repository) :
            base(repository)
        {

        }

        protected override string GetPageUrl(string format, Dictionary<string,object> arg)
        {
            var displayCarInPage = 15;
            var radius = "-1";
            var url = "http://www.autotrader.ca/";
            return string.Format(
                        "{3}/cars/?prx={0}&rcs={1}&rcp={2}&adtype=Dealer&sts=New&showcpo=1&hprc=True&wcp=False",
                        radius, (int)arg["page"] * displayCarInPage, displayCarInPage, url);
        }

        #region ForDebugOnly
        protected override void SecondPhase(List<ParssedCar> parrsedCars, List<Field> fields)
        {
            var htmlWeb = new HtmlWeb();
            foreach (var parrsedCar in parrsedCars)
            {
                var htmlDocument = GetHtmlDocument2();
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

        protected override HtmlDocument GetHtmlDocument(HtmlWeb htmlWeb, string url)
        {
            var sourceDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            var path = System.IO.Path.Combine(sourceDirectory, "autotrader\\New & Used Cars for sale in Ontario _ autoTRADER.ca.html");
            var html = System.IO.File.ReadAllText(path);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

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