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
            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Cookie, @".ASPXANONYMOUS=h6x2brvS0gEkAAAANzc1OGRhYzMtZjZmOC00MGRmLTg4YjUtZjE2YjlkNTI3ZGVl6KRq9zbUNS3PwEeJp2YXVHB-G_lCijwYB80r_3S5nGs1;;___utmvc=navigator%3Dtrue,navigator.vendor%3D,navigator.appName%3DNetscape,navigator.plugins.length%3D%3D0%3Dfalse,navigator.platform%3DWin32,navigator.webdriver%3Dfalse,plugin_ext%3Docx,ActiveXObject%3Dfalse,webkitURL%3Dfalse,_phantom%3Dfalse,callPhantom%3Dfalse,chrome%3Dfalse,yandex%3Dfalse,opera%3Dfalse,opr%3Dfalse,safari%3Dfalse,awesomium%3Dfalse,puffinDevice%3Dfalse,__nightmare%3Dfalse,_Selenium_IDE_Recorder%3Dfalse,document.__webdriver_script_fn%3Dfalse,document.%24cdc_asdjflasutopfhvcZLmcfl_%3Dfalse,process.version%3Dfalse,navigator.cpuClass%3Dtrue,navigator.oscpu%3Dfalse,navigator.connection%3Dfalse,window.outerWidth%3D%3D0%3Dfalse,window.outerHeight%3D%3D0%3Dfalse,window.WebGLRenderingContext%3Dtrue,document.documentMode%3D11,eval.toString().length%3D39,digest=75385,75514;___utmvmNmufwBX=BgClKyBuURt;___utmvaNmufwBX=CoS_nToO;___utmvbNmufwBX=SZm XrlOhalH: ztY;visid_incap_820541=baKA9kdrTLusvwaOHgf4/R7oxlgAAAAAQUIPAAAAAAAxQ7Z4qvsByD2+6VebkibP;uag=9FA9D5EEDE64FA7B926FF39E7680599019B2077654E1337086ECAE6E68338124;at_uid=HH%2fn1CwDk0u4VX7SxdKdaQ%3d%3d;visid_incap_820268=jPoRC4qjS7u5y9RNdTAVvB/oxlgAAAAAQUIPAAAAAADQZoRKpA6oxHEgU9pnguVN;_ga=GA1.2.1446513763.1489430572;_cplid=3ad3e86749f7ba301a08926a288aed2715ac8faed81;__utma=1.1446513763.1489430572.1489430580.1489430580.1;__utmz=1.1489430580.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none);__utmv=1.test|8=HH%2Fn1CwDk0u4VX7SxdKdaQ%3D%3D=User%20ID=1;__gads=ID=76472c63f2948495:T=1489430576:S=ALNI_MbE888PR7CLRFFd-MqhZUwQW_uBtg;_fsspl_=%7B%22when%22%3A1489433545772%2C%22keys%22%3A%7B%22rid%22%3A%7B%22v%22%3A%22916b9f766af526298ebb50052775851c%22%2C%22x%22%3A1497206586002%7D%2C%22cp%22%3A%7B%22v%22%3A%7B%22url%22%3A%22http%3A%2F%2Fwwwb.autotrader.ca%2Fcars%2Fbuick%2F%3Fprx%3D-1%26rcs%3D0%26rcp%3D100%26adtype%3DDealer%26sts%3DNew%26showcpo%3D1%26hprc%3DTrue%26wcp%3DFalse%22%2C%22isLoggedIn%22%3A%22N%22%2C%22HasContactedSeller%22%3A%22N%22%2C%22terms%22%3A%22%22%2C%22browser%22%3A%22IE%2011%22%2C%22os%22%3A%22Windows%22%2C%22referrer%22%3A%22%22%2C%22site%22%3A%22autotrader.ca%22%2C%22code%22%3A%2219.1.2%22%2C%22fp%22%3A%22undefined%22%2C%22pv%22%3A%221%22%2C%22locale%22%3A%22en%22%7D%2C%22x%22%3A1497206586910%7D%2C%22pl%22%3A%7B%22v%22%3A1%2C%22x%22%3A1489444986914%2C%22ttl%22%3A14400000%7D%2C%22pv%22%3A%7B%22v%22%3A1%2C%22x%22%3A1497206586915%7D%2C%22def%22%3A%7B%22v%22%3A0%2C%22x%22%3A1497206586917%7D%2C%22browsepv%22%3A%7B%22v%22%3A1%2C%22x%22%3A1497206586918%7D%7D%7D;249_MVT=Production;visid_incap_820528=aFdrRYXpSXGv4slyqrqMtK72FlkAAAAAQUIPAAAAAAAFae066NfXzVlW/vEA9Xg3;incap_ses_143_820528=0KyaZu2PCFdPRFa7+gr8Aa72FlkAAAAAbfrjEJoRxBEpP1NCh5+4jg==;incap_ses_586_820541=+2oNLMyhQRBoAXtnGOQhCLP2FlkAAAAAF8AWkqkOFPq9Gqqv/z53Gw==");
            //wc.Proxy = new WebProxy("137.74.254.198", 3128);
            for (var i = 0; i < NumberOfRetries; i++)
            {
                try
                {
                    //return htmlWeb.Load(url);
                    var page = wc.DownloadString(url);

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(page);
                    return doc;
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
            var abc = htmlDocument.DocumentNode.SelectNodes(listField.Xpath) ??
                      htmlDocument.DocumentNode.SelectNodes("//*[@id='adList']/div/div[contains(@class, 'at_result')]");
            var carListNodes = abc.ToList();
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

            //Добавление MSRP
            //var list = new List<FieldValue>();
            //var msrpFiled = fields.First(a => a.Name == FiledNameConstant.MSRP);
            //foreach (var savedParsedCar in savedParsedCars)
            //{
            //    var parsedCar = parsedCars.FirstOrDefault(a => a.ForCompare == savedParsedCar.ForCompare);
            //    var msrp = savedParsedCar.FieldValues.FirstOrDefault(a => a.Field.Name == FiledNameConstant.MSRP);
            //    if (parsedCar != null && msrp == null)
            //    {
            //        var field = parsedCar.FieldValues.FirstOrDefault(a => a.Field.Name == FiledNameConstant.MSRP);
            //        if (field == null)
            //        {
            //            list.Add(new FieldValue
            //            {
            //                Field = msrpFiled,
            //                ParsedCarId = savedParsedCar.Id,
            //                Value = prices.FirstOrDefault(a => a.ParsedCarId == savedParsedCar.Id).Value
            //            });
            //        }
            //        else
            //        {
            //            list.Add(new FieldValue
            //            {
            //                Field = msrpFiled,
            //                ParsedCarId = savedParsedCar.Id,
            //                Value = field.Value
            //            });
            //        }
            //    }
            //}
            //if (list.Any())
            //{
            //    Repository.AddFieldValues(list);
            //}

            return result;
        }

        protected virtual ParsedCar ParseCarNode(IEnumerable<Field> fields, HtmlNode carListNode)
        {
            //var urlField = ;
            var urlFieldValue = carListNode.SelectSingleNode(".//h2/a").GetAttributeValue("href", string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(urlFieldValue))
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
                Url = urlFieldValue.Split('?')[0],

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

