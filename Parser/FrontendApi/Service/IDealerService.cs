using System.Collections.Generic;
using FrontendApi.Models;

namespace FrontendApi.Service
{
    public interface IDealerService
    {
        List<CarViewModel> GetCarsByDealerId(int dealerId);
        void InitDb(Dealer dealer);
        DealerClassInformation GetInformationById(int dealerCarId);
        ChartData GetChartDataById(int stockCarId, int dealerId, int carId);
        List<DealerCompetitorCar> GetDealerCompetitorsById(int stockCarId, int dealerId, int carId);
        ChartSeries GetPriceTrendById(int stockCarId, int carId);
        DealerInformation GetDealer(int id);
        ChartSeries GetCountTrendById(int stockCarId);
    }
}
