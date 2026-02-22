namespace NorthwindAPI_2.DTO
{
    public class EmployeePlaceOrderRequest
    {
        public int EmployeeId { get; set; }
        public List<EmployeeNewOrderRequestDTO> OrderItems { get; set; }
    }
}
