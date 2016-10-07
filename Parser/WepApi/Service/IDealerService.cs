using System.Collections.Generic;
using WepApi.Models;

namespace WepApi.Service
{
    public interface IDealerService
    {
        List<CarViewModel> GetCarsByDealerId(int dealerId);
        void InitDb(Dealer dealer);
        DealerClassInformation GetInformationById(int dealerCarId);
        ChartData GetChartDataById(int stockCarId);
        List<DealerCompetitorCar> GetDealerCompetitorsById(int stockCarId, int dealerId);
        ChartSeries GetPriceTrendById(int stockCarId);
    }
}
