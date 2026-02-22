using Microsoft.AspNetCore.Mvc;
using NorthwindAPI_2.DTO;
using NorthwindAPI_2.Services;

namespace NorthwindAPI_2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DapperController : ControllerBase
    {

        public readonly IDapperSalesService _iDapperSalesService;

        public DapperController(IDapperSalesService iDapperSalesService) => _iDapperSalesService = iDapperSalesService;


        [HttpGet("GetPerformanceList")]
        public async Task<IActionResult> GetPerformanceReportList(int year, string categoryName)
        {

            var result = await _iDapperSalesService.GetPerformanceReport(year, categoryName);
            return Ok(result);


        }

        [HttpPost("EmployeeOrder")]
        public async Task<IActionResult> EmployeeOrder([FromBody] EmployeePlaceOrderRequest request)
        {

            string result = await _iDapperSalesService.EmployeeMakeOrder(request);
          
            if(result== "Product Not Available" || result == "Employee Verification Failed")
            {
                return BadRequest(result);
            }

            return Ok(result);

        }



    }
}
