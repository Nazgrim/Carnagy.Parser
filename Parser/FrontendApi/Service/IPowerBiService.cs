using System.Threading.Tasks;
using FrontendApi.Models;

namespace FrontendApi.Service
{
    public interface IPowerBiService
    {
        PBIReports GetReports(string accessToken);
        PBIDashboards GetDashboards(string accessToken);
        Task<AzureAdTokenResponse> GetToken();
    }
}