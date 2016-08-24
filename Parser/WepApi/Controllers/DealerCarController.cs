using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using WepApi.Models;

namespace WepApi.Controllers
{
    public class DealerCarController : ApiController
    {
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

        #endregion
    }

}
