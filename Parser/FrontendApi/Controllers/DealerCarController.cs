using System.Collections.Generic;
using FrontendApi.Models;
using FrontendApi.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FrontendApi.Controllers
{
    public class DealerCarController : Controller
    {
        private readonly IDealerService _dealerService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DealerCarController(IDealerService dealerService, IHostingEnvironment hostingEnvironment)
        {
            _dealerService = dealerService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [ActionName("Information")]
        public IActionResult GetCarInformation(int carId)
        {
            var information = _dealerService.GetInformationById(carId);
            if (information == null)
            {
                return NotFound();
            }
            return Ok(information);
        }

        [HttpGet]
        [ActionName("ChartData")]
        public IActionResult GetChartData(int stockCarId, int dealerId, int carId)
        {
            var chartData = _dealerService.GetChartDataById(stockCarId, dealerId, carId);
            if (chartData == null)
            {
                return NotFound();
            }
            return Ok(chartData);
        }

        [HttpGet]
        [ActionName("DealerCompetitors")]
        public IActionResult GetDealerCompetitors(int stockCarId, int deaerId = 1, int carId=0)
        {
            var dealerCompetitors = _dealerService.GetDealerCompetitorsById(stockCarId, deaerId, carId);
            if (dealerCompetitors == null)
            {
                return NotFound();
            }
            return Ok(dealerCompetitors);
        }

        [HttpGet]
        [ActionName("ChartSeries")]
        public IActionResult GetChartSeries(int stockCarId, int carId)
        {
            var priceTrend = _dealerService.GetPriceTrendById(stockCarId, carId);
            if (priceTrend == null)
            {
                return NotFound();
            }
            return Ok(priceTrend);
        }

        [HttpGet]
        [ActionName("CountTrend")]
        public IActionResult GetCountTrend(int stockCarId)
        {
            var countTrend = _dealerService.GetCountTrendById(stockCarId);
            if (countTrend == null)
            {
                return NotFound();
            }
            return Ok(countTrend);
        }


        [HttpGet]
        [ActionName("SimilarCars")]
        public IActionResult GetSimilarCars(int dealerCarId)
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
            var filePath = _hostingEnvironment.ContentRootPath + "~/Files/similarCars.json";
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<List<SimilarCar>>(json);
            return result;
        }
        #endregion
    }

}