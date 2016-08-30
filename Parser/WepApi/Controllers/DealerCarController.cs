using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using WepApi.Models;

namespace WepApi.Controllers
{
    public class DealerCarController : ApiController
    {
        [HttpGet]
        [ActionName("DealerCar")]
        public IHttpActionResult GetDealerCar(int id)
        {
            var dealerCar = GetDelearCarById(id);
            if (dealerCar == null)
            {
                return NotFound();
            }
            return Ok(dealerCar);
        }

        [HttpGet]
        [ActionName("Information")]
        public IHttpActionResult GetCarInformation(int dealerCarId)
        {
            var information = GetInformationById(dealerCarId);
            if (information == null)
            {
                return NotFound();
            }
            return Ok(information);
        }

        [HttpGet]
        [ActionName("ChartData")]
        public IHttpActionResult GetChartData(int dealerCarId)
        {
            var chartData = GetChartDataById(dealerCarId);
            if (chartData == null)
            {
                return NotFound();
            }
            return Ok(chartData);
        }

        [HttpGet]
        [ActionName("SimilarCars")]
        public IHttpActionResult GetSimilarCars(int dealerCarId)
        {
            var similarCars = GetSimilarCarsById(dealerCarId);
            if (similarCars == null)
            {
                return NotFound();
            }
            return Ok(similarCars);
        }
        [HttpGet]
        [ActionName("PriceTrend")]
        public IHttpActionResult GetPriceTrend(int dealerCarId)
        {
            var priceTrend = GetPriceTrendById(dealerCarId);
            if (priceTrend == null)
            {
                return NotFound();
            }
            return Ok(priceTrend);
        }
        [HttpGet]
        [ActionName("DealerCompetitors")]
        public IHttpActionResult GetDealerCompetitors(int dealerCarId)
        {
            var dealerCompetitors = GetDealerCompetitorsById(dealerCarId);
            if (dealerCompetitors == null)
            {
                return NotFound();
            }
            return Ok(dealerCompetitors);
        }

        #region ForTestOnly

        private DealerCar GetDelearCarById(int id)
        {
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/dealerCar.json");
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<DealerCar>(json);
            return result;
        }

        private DealerClassInformation GetInformationById(int dealerCarId)
        {
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/carInformation.json");
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<DealerClassInformation>(json);
            return result;
        }

        private ChartData GetChartDataById(int dealerCarId)
        {
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/chartData.json");
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<ChartData>(json);
            return result;
        }

        private List<SimilarCar> GetSimilarCarsById(int dealerCarId)
        {
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/similarCars.json");
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<List<SimilarCar>>(json);
            return result;
        }

        private PriceTrend GetPriceTrendById(int dealerCarId)
        {
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/priceTrend.json");
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<PriceTrend>(json);
            return result;
        }

        private List<DealerCompetitor> GetDealerCompetitorsById(int dealerCarId)
        {
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/dealerCompetitor.json");
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<List<DealerCompetitor>>(json);
            return result;
        }


        private void GetAreas()
        {
            var rnd = new System.Random();
            var carDeales = new List<int>();
            for (int i = 0; i < 40; i++)
            {
                carDeales.Add(rnd.Next(0, 1000));
            }

            var min = carDeales.Min();
            var max = carDeales.Max();
            var minMaxDif = max - min;
            var areaCount = 4;//количество областей
            carDeales = carDeales.OrderBy(a => a).ToList();
            var increment = minMaxDif / areaCount;
            var previousValue = 0;
            var result = new List<int>();
            for (int i = increment; i <= minMaxDif; i += increment)
            {
                var areaCars = carDeales.Where(a => a > previousValue && a <= i).OrderBy(a => a).ToList();
                var minX = areaCars.Min();
                var maxX = areaCars.Max();
                var minMaxDifX = max - min;
                var areaCountX = 20;//количество областей
                var incrementX = minMaxDif / areaCount;
                var previousValueX = 0;
                for (int j = incrementX; j <= minMaxDifX; j += incrementX)
                {
                    var count = areaCars.Where(a => a > previousValueX && a <= i).Count();
                    result.Add(count);
                    previousValueX = j;
                }

                previousValue = i;
            }



            var str = string.Join(",", result);

        }

        #endregion
    }

}