using DataAccess;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Runner
{
    public class DealerParser
    {
        private const string UrlPattern = "http://www.autotrader.ca/dealer/dealerfinder/DealerFinder.aspx?rctry=true&rcs={0}";
        private const int NumberOfRetries = 3;
        private const int DelayOnRetry = 1000;
        public void Parse()
        {
            var step = 35;
            var max = 6125;
            var pages = Enumerable.Range(0, max / step + 1).Select(x => x * step);
            var dealerList = new List<Dealer>();
            var htmlWeb = new HtmlWeb();
            var list = new List<DealerLogItem>();
            var lisrError = new List<string>();
            Parallel.ForEach(pages, page =>
            //foreach (var page in pages)
            {
                var logItemCollection = new List<DealerLogItem>();
                var currentId = 0;
                try
                {
                    var url = string.Format(UrlPattern, page);
                    var htmlDocumentResult = GetFromUrl(htmlWeb, url);
                    if (htmlDocumentResult != null)
                    {
                        logItemCollection.Add(new DealerLogItem { Page = page, Id = currentId, HtmlItem = url });
                        currentId++;
                        //var path = @"C:\Users\Dima Smirnov\Desktop\Новая папка\Dealer Finder  Local Dealer Financing, Rebates and Service Centers - autoTRADER.ca.htm";
                        //var html = File.ReadAllText(path);
                        //var htmlDocument = new HtmlDocument();
                        //htmlDocument.LoadHtml(html);
                        var dealerDivs =
                   htmlDocumentResult.HtmlDocument.DocumentNode.SelectSingleNode("//*[@id='dealerfinder']/div[3]/div[3]")
                       .ChildNodes.Where(a => a.Name == "div");
                        foreach (var dealerDivDiv in dealerDivs)
                        {
                            var log = new DealerLogItem { Page = page, Id = currentId, HtmlItem = dealerDivDiv.XPath };
                            logItemCollection.Add(log);
                            dealerList.Add(ParseDealer(dealerDivDiv));
                            currentId++;
                        }
                        list.AddRange(logItemCollection);
                    }
                }
                catch (Exception e)
                {
                    lisrError.Add($"page:{page};currentId:{currentId};errorMEssage:{e.Message}");
                    //var logItem = logItemCollection.SingleOrDefault(a => a.Id == currentId);
                    //if (logItem != null)
                    //{
                    //    logItem.Exeption = e.Message;
                    //}
                }
                Thread.Sleep(DelayOnRetry);
            }
            );
            File.WriteAllLines("dealerError.json", new[] { JsonConvert.SerializeObject(lisrError) });
            File.WriteAllLines("dealer1.json", new[] { JsonConvert.SerializeObject(dealerList) });
        }

        public void FixLocation(CarnagyContext context)
        {
            string json = File.ReadAllText("dealer1.json");
            var dealersFromJson = JsonConvert.DeserializeObject<List<Dealer>>(json);

            //First Phase
            //var dealersFromBD = context.Dealers.Include(a => a.Cars).Where(a => a.Cars.Any(b => b.StockCar.MakeId == 3)).ToList();
            //var dealers1 = dealersFromBD.Where(a=>a.Name== "Myers Kanata Chev Buick GMC.");
            //foreach (var item in dealers1)
            //{
            //    item.Location = "Ottawa, ON";
            //}
            //var dealers2 = dealersFromBD.Where(a=>a.Name== "City Buick Chevrolet Cadillac GMC");
            //foreach (var item in dealers2)
            //{
            //    item.Location = "Toronto, ON";
            //}
            //var dealers3 = dealersFromBD.Where(a => a.Name == "Applewood Chevrolet Cadillac Buick GMC");
            //foreach (var item in dealers3)
            //{
            //    item.Location = "Mississauga, ON";
            //}
            //var dealers4 = dealersFromBD.Where(a => a.Name == "addisongm");
            //foreach (var item in dealers4)
            //{
            //    item.Location = "Mississauga, ON";
            //}

            //Second Phase
            //var dealersFromBD = context.Dealers.Where(a=>a.Location==null).Include(a => a.Cars).Where(a => a.Cars.Any(b => b.StockCar.MakeId == 3)).ToList();
            //foreach (var dealer in dealersFromBD)
            //{
            //    dealer.Name = dealer.Name.Replace("&amp;", "&");
            //    dealer.Name = dealer.Name.Replace("&#39;", "'");
            //}

            var dealersFromBD = context.Dealers.Where(a=>a.Location==null).Include(a => a.Cars).Where(a => a.Cars.Any(b => b.StockCar.MakeId == 3)).ToList();
            foreach (var dealer in dealersFromBD)
            {
                if (dealer.Name=="Murray Motors The Pas") {
                    dealer.Location = "The Pas, MB";
                }

                if (dealer.Name == "Grenier Chevrolet Buick GMC")
                {
                    dealer.Location = "Terrebonne, QC";
                    dealer.WebSireUrl = "https://www.grenierchevrolet.com/";
                }

                if (dealer.Name == "Mike Jackson GM")
                {
                    dealer.Location = "Collingwood, ON";
                }
            }
            var groupDelears = dealersFromBD.Where(a=>a.Location==null).GroupBy(a => a.Name);

            var list1 = new List<Dealer>();

            //foreach (var dealers in groupDelears)
            //{
            //    var dealerFromJson1 = dealersFromJson.Where(a => a.Name == dealers.Key);
            //    if (dealerFromJson1.Any())
            //    {
            //        list1.AddRange(dealerFromJson1);
            //        foreach (var dealer in dealers)
            //        {
            //            var test123 = dealerFromJson1.First();
            //            dealer.Location = test123.CityName + ", " + test123.Province;
            //        }
            //    }
            //    //var dealerFromJson2 = dealersFromJson.Where(a => !string.IsNullOrWhiteSpace(a.DealerUrl))
            //    //    .Where(a => dealer.Select(b => b.WebSireUrl).Contains(a.DealerUrl));

            //    //if (dealerFromJson2.Any())
            //    //    list2.AddRange(dealerFromJson2);
            //}

            
            //var list2 = new List<Dealer>();
            //foreach (var dealers in groupDelears)
            //{
                
            //    var dealerFromJson2 = dealersFromJson.Where(a => !string.IsNullOrWhiteSpace(a.DealerUrl))
            //        .Where(a => dealers.Select(b => b.WebSireUrl).Contains(a.DealerUrl));

            //    if (dealerFromJson2.Any())
            //    {
            //        list2.AddRange(dealerFromJson2);
            //        foreach (var dealer in dealers)
            //        {
            //            var test123 = dealerFromJson2.First();
            //            dealer.Location = test123.CityName + ", " + test123.Province;
            //        }
            //    }
            //}

            context.SaveChanges();

            //var abc4 = string.Join("\n", dealersFromJson.Select(a => a.DealerUrl).ToArray());
            //var list3 = new List<Dealer>();
            //list3.AddRange(list1);
            //list3.AddRange(list2);
            //var test = list3.GroupBy(a => a.Name).ToList();
            //var abc3 = string.Join("\n", test.Select(a => a.Key).OrderBy(a => a).ToArray());
        }

        public void CreateCsv(CarnagyContext context)
        {
           var dealersGroup= context.Dealers.Include(a => a.Cars).ToList().GroupBy(a=>a.Location);
            var list = new List<string>
           {
               "sep=^",
               "Location",
               "Count"
           };
            foreach (var dealers in dealersGroup)
            {
                list.Add($"{dealers.Key}^{dealers.SelectMany(a => a.Cars).Count(a => a.StockCar.MakeId == 3 && !a.MainAdvertCar.IsDeleted)}");
            }
            File.WriteAllLines("location.csv", list.ToArray());
        }

        private static HtmlDocumentResult GetFromUrl(HtmlWeb htmlWeb, string url)
        {
            var result = new HtmlDocumentResult();
            for (var i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    result.HtmlDocument = htmlWeb.Load(url);
                    return result;
                }
                catch (WebException ex)
                {
                    result.LogError += string.Format("Status:{0}.Message:{1}:InnerExeption{2}" + Environment.NewLine, ex.Status, ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                    if (i == NumberOfRetries)
                        return result;
                    Thread.Sleep(DelayOnRetry);
                }
                catch (Exception ex)
                {
                    result.LogError += string.Format("Message:{0}:InnerExeption{1}" + Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                    return result;
                }
            }
            return null;
        }

        private static Dealer ParseDealer(HtmlNode dealerDivDiv)
        {
            var dealer = new Dealer();
            var dealerDiv = dealerDivDiv.ChildNodes.Where(a => a.Name == "div").ToList();

            var infoArray =
                dealerDiv[1].InnerText.Split(new[] { '\n' }, StringSplitOptions.None)
                    .Select(a => a.Trim())
                    .Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
            var cityAndProvince = infoArray[1].Split(' ');
            dealer.Name = dealerDiv[0].InnerText;
            dealer.Adress = infoArray[0];
            dealer.CityName = cityAndProvince[0];
            dealer.Province = cityAndProvince[1].Trim().Replace("(", string.Empty).Replace(")", string.Empty);
            dealer.ZipCode = infoArray[2];
            dealer.Phone = infoArray[3].Split(' ')[1];
            var webSiteDiv = dealerDiv[4].ChildNodes.Where(a => a.Name == "a").ToList();
            if (webSiteDiv.Any())
            {
                var websiteLink = webSiteDiv.FirstOrDefault(a => a.InnerText == "Visit Dealer Website");
                if (websiteLink != null) dealer.DealerUrl = websiteLink.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrWhiteSpace(dealer.DealerUrl))
                {
                    dealer.DealerUrl = dealer.DealerUrl.Split('?')[0];
                }
            }
            var logoDiv = dealerDiv[3].ChildNodes.Where(a => a.Name == "a").ToList();
            dealer.DealerLogo = logoDiv[0].ChildNodes[0].GetAttributeValue("src", string.Empty);

            return dealer;
        }
        public class HtmlDocumentResult
        {
            public HtmlDocument HtmlDocument { get; set; }
            public string LogError { get; set; }
        }

        public class Dealer
        {
            public string Name { get; set; }
            public string DealerLogo { get; set; }
            public string DealerUrl { get; set; }
            public string Adress { get; set; }
            public string CityName { get; set; }
            public string ZipCode { get; set; }
            public string Phone { get; set; }
            public string Province { get; set; }

            public bool IsFullInformation { get; set; }
            public int Id { get; set; }
            public int Value { get; set; }
            public int CarsCount { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string DealerPlace { get; set; }
        }

        public class DealerLogItem
        {
            public int Id { get; set; }
            public int Page { get; set; }
            public string HtmlItem { get; set; }
            public string Exeption { get; set; }
        }
    }
}
