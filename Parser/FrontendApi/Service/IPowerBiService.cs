using System.Threading.Tasks;
using FrontendApi.Models;

namespace FrontendApi.Service
{
    public interface IPowerBiService
    {
        PBIDashboards GetDashboards(string accessToken);
        Task<AzureAdTokenResponse> GetToken();
    }
}