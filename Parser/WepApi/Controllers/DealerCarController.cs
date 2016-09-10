using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using WepApi.Models;
using WepApi.Service;

namespace WepApi.Controllers
{
    public class DealerCarController : ApiController
    {
        private IDealerService DealerService { get; set; }

        public DealerCarController(IDealerService dealerService)
        {
            DealerService = dealerService;
        }

        [HttpGet]
        [ActionName("Information")]
        public IHttpActionResult GetCarInformation(int dealerCarId)
        {
            var information = DealerService.GetInformationById(dealerCarId);
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
            var chartData = DealerService.GetChartDataById(dealerCarId);
            if (chartData == null)
            {
                return NotFound();
            }
            return Ok(chartData);
        }

        [HttpGet]
        [ActionName("DealerCompetitors")]
        public IHttpActionResult GetDealerCompetitors(int stockCarId, int deaerId = 1)
        {
            var dealerCompetitors = DealerService.GetDealerCompetitorsById(stockCarId, deaerId);
            if (dealerCompetitors == null)
            {
                return NotFound();
            }
            return Ok(dealerCompetitors);
        }

        [HttpGet]
        [ActionName("ChartSeries")]
        public IHttpActionResult GetChartSeries(int stockCarId)
        {
            var priceTrend = DealerService.GetPriceTrendById(stockCarId);
            if (priceTrend == null)
            {
                return NotFound();
            }
            return Ok(priceTrend);
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

        #region ForTestOnly
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
        #endregion
    }

}