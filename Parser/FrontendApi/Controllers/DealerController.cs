using System.Collections.Generic;
using System.Linq;
using FrontendApi.Models;
using FrontendApi.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FrontendApi.Controllers
{
    public class DealerController : Controller
    {
        private readonly IDealerService _dealerService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DealerController(IDealerService dealerService, IHostingEnvironment hostingEnvironment)
        {
            _dealerService = dealerService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [ActionName("DealerCars")]
        public IActionResult GetDealer(int id)
        {
            var dealerCars = _dealerService.GetCarsByDealerId(id);
            if (dealerCars == null)
            {
                return NotFound();
            }
            return Ok(dealerCars);
        }

        [HttpGet]
        [ActionName("DealerInformation")]
        public IActionResult GetDealerInformation(int id)
        {
            var dealer = _dealerService.GetDealer(id);
            
            if (dealer == null)
            {
                return NotFound();
            }
            return Ok(dealer);
        }



        #region ForTestOnly

        [HttpGet]
        [ActionName("InitDb")]
        public IActionResult InitDb()
        {
            var dealer = GetDelearById(1);

            _dealerService.InitDb(dealer);

            return Ok();
        }

        private Dealer GetDelearById(int id)
        {
            return GetDealers().FirstOrDefault();
        }

        private List<Dealer> GetDealers()
        {
            var filePath = _hostingEnvironment.ContentRootPath + "~/Files/dealers.json";
            string json = System.IO.File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<List<Dealer>>(json);
            return result;
        }

        #endregion
    }
}
