namespace NorthwindAPI_2.DTO
{
    public class FinalPerfomanceReport
    {
        public List<PerformanceReportDTO> MainReport { get; set; }
        public int TotalOrdersHandled { get; set; }
        public int TopSellingProductInCategory { get; set; }

    }
}
