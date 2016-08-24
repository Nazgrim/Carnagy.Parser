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

        #endregion
    }

}
