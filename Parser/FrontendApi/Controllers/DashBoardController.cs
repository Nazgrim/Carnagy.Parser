using System.Linq;
using System.Threading.Tasks;
using FrontendApi.Models;
using FrontendApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FrontendApi.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly IPowerBiService _powerBiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public DashBoardController(IPowerBiService powerBiService, IHttpContextAccessor httpContextAccessor)
        {
            _powerBiService = powerBiService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [ActionName("Token")]
        public async Task<IActionResult> GetToken()
        {
            var token = _session.GetObjectFromJson<AzureAdTokenResponse>("token") ?? await _powerBiService.GetToken();

            if (token == null)
            {
                return NotFound();
            }
            _session.SetObjectAsJson("token", token);
            return Ok(token);
        }

        [HttpGet]
        [ActionName("DashBoardUrl")]
        public IActionResult GetDashBoardUrl(string accessToken)
        {
            var dasboards = _powerBiService.GetDashboards(accessToken);

            if (dasboards == null || !dasboards.value.Any())
            {
                return NotFound();
            }
            return Ok(dasboards.value.First());
        }
    }

    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
