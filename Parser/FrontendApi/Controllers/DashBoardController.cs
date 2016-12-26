using System.Linq;
using System.Threading.Tasks;
using FrontendApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace FrontendApi.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly IPowerBiService _powerBiService;
        public DashBoardController(IPowerBiService powerBiService)
        {
            _powerBiService = powerBiService;
        }

        [HttpGet]
        [ActionName("DashBoardUrl")]
        public async Task<IActionResult> GetDashBoardUrl()
        {
            var token = await _powerBiService.GetToken();
            var dasboards = _powerBiService.GetDashboards(token.AccessToken);

            if (dasboards == null || !dasboards.value.Any())
            {
                return NotFound();
            }
            return Ok(dasboards.value.First());
        }
    }
}
