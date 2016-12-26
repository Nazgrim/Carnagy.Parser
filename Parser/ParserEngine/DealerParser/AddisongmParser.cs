using DataAccess.Repositories;
using HtmlAgilityPack;
using Utility;

namespace ParserEngine.DealerParser
{
    public class AddisongmParser : BaseParser
    {
        public AddisongmParser(IParseRepository repository) :
            base(repository, "addisongm")
        {

        }

        //private HtmlDocument GetHtmlDocument2)
        //{
        //    var sourceDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
        //    var path = System.IO.Path.Combine(sourceDirectory, "autotrader\\1970 Chevrolet Chevelle SS for $65,000 in KITCHENER _ autoTRADER.ca.html");
        //    var html = System.IO.File.ReadAllText(path);
        //    var htmlDocument = new HtmlDocument();
        //    htmlDocument.LoadHtml(html);
        //    return htmlDocument;
        //}

    }
}
