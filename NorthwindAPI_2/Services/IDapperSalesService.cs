using NorthwindAPI_2.DTO;

namespace NorthwindAPI_2.Services
{
    public interface IDapperSalesService
    {
        Task<FinalPerfomanceReport> GetPerformanceReport(int year, string categoryName);
        Task<string> EmployeeMakeOrder(EmployeePlaceOrderRequest request);
    }
}
