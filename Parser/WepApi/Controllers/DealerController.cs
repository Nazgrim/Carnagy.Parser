using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WepApi.Models;
using WepApi.Service;

namespace WepApi.Controllers
{
    public class DealerController : ApiController
    {
        private IDealerService DealerService { get; set; }

        public DealerController(IDealerService dealerService)
        {
            DealerService = dealerService;
        }
        
        [HttpGet]
        [ActionName("DealerCars")]
        public IHttpActionResult GetDealer(int id)
        {
            var dealerCars = DealerService.GetCarsByDealerId(id);
            if (dealerCars == null)
            {
                return NotFound();
            }
            return Ok(dealerCars);
        }

        

        #region ForTestOnly

        [HttpGet]
        [ActionName("InitDb")]
        public IHttpActionResult InitDb()
        {
            var dealer = GetDelearById(1);

            DealerService.InitDb(dealer);

            return Ok();
        }

        private Dealer GetDelearById(int id)
        {
            return GetDealers().FirstOrDefault();
        }

        private List<Dealer> GetDealers()
        {
            var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Files/dealers.json");
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<List<Dealer>>(json);
            return result;
        }

        #endregion
    }
}
