namespace DataAccess.Models
{
    public enum ParsedCarStatus
    {
        None = 0,
        List = 2,
        LoadPageError = 4,
        Page = 8,
        CantGetStockCar = 16,
        NoDealerWebSite = 32,
        AnalyzeComplete = 64,
        CannotParsePrice = 128
    }
}
