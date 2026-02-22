namespace NorthwindAPI_2.DTO
{
    public class PerformanceReportDTO
    {
        public string EmployeeFullName { get; set; }
        public decimal RevenueWithoutDiscount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalRevenueGenerated { get; set; }
      
    }
}
