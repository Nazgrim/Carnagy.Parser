using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WepApi.Models;

namespace WepApi.Controllers
{
    public class DealerController : ApiController
    {
        public IHttpActionResult GetDealer(int id)
        {
            var dealer = GetDelearById(id);
            if (dealer == null)
            {
                return NotFound();
            }
            return Ok(dealer);
        }

        private Dealer GetDelearById(int id)
        {
            return GetDealers().FirstOrDefault();             
        }

        #region ForTestOnly

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
