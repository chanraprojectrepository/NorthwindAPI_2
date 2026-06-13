using System.Data;
using Dapper;
using NorthwindAPI_2.DTO;

namespace NorthwindAPI_2.Services
{
    public class DapperSalesService : IDapperSalesService
    {

        private readonly IDbConnection _db;

        public DapperSalesService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<FinalPerfomanceReport> GetPerformanceReport(int year, string categoryName)
        {
            int totalOrderReportToInt = 0;
            int topSellingProductsToInt = 0;

            //EmployeeFullName,TotalRevenueGenerated details
            string sqlQueryMain = @"SELECT 
            e.FirstName + e.LastName AS EmployeeFullName,
            od.UnitPrice * od.Quantity AS RevenueWithoutDiscount,
            od.Discount * (od.UnitPrice * od.Quantity) AS DiscountAmount,
            p.ProductName
            FROM Employees e
            INNER JOIN Orders o ON e.EmployeeID = o.EmployeeID
            INNER JOIN [Order Details] od ON o.OrderID = od.OrderID
            INNER JOIN Products p ON od.ProductID = p.ProductID
            INNER JOIN Categories c ON p.CategoryID = c.CategoryID
            WHERE c.CategoryName=@CatName AND YEAR(o.OrderDate)=@Year";

            //TotalOrdersHandled
            string sqlQueryTotalOrders = @"SELECT 
            COUNT(o.OrderID) AS TotalOrdersHandled
            FROM Employees e
            INNER JOIN Orders o ON e.EmployeeID = o.EmployeeID
            INNER JOIN [Order Details] od ON o.OrderID = od.OrderID
            INNER JOIN Products p ON od.ProductID = p.ProductID
            INNER JOIN Categories c ON p.CategoryID = c.CategoryID
            WHERE c.CategoryName=@CatName AND YEAR(o.OrderDate)=@Year";

            //TopSellingProductInCategory
            string sqlQueryTopSellingProduct = @"SELECT 
            MAX(od.Quantity) AS TopSellingProductInCategory
            FROM Employees e
            INNER JOIN Orders o ON e.EmployeeID = o.EmployeeID
            INNER JOIN [Order Details] od ON o.OrderID = od.OrderID
            INNER JOIN Products p ON od.ProductID = p.ProductID
            INNER JOIN Categories c ON p.CategoryID = c.CategoryID
            WHERE c.CategoryName=@CatName AND YEAR(o.OrderDate)=@Year";


            var mainResult = await _db.QueryAsync<PerformanceReportDTO>(sqlQueryMain, new
            {
                CatName = categoryName,
                Year = year
            });

            var totalOrdersHandledResult = await _db.QueryAsync<int>(sqlQueryTotalOrders, new
            {
                CatName = categoryName,
                Year = year
            });

            var topSellingProductInCategoryResult = await _db.QueryAsync<int>(sqlQueryTopSellingProduct, new
            {
                CatName = categoryName,
                Year = year
            });

            var mainReports = mainResult.ToList();
            var totalOrderReports = totalOrdersHandledResult.ToList();
            var topSellingProducts = topSellingProductInCategoryResult.ToList();

            foreach (var mainReport in mainReports)
            {
                mainReport.TotalRevenueGenerated = mainReport.RevenueWithoutDiscount - mainReport.DiscountAmount;
            }

            foreach (var totalOrderReport in totalOrderReports)
            {
                totalOrderReportToInt = totalOrderReport;
            }

            foreach (var topSellingProduct in topSellingProducts)
            {
                topSellingProductsToInt = topSellingProduct;
            }

            return new FinalPerfomanceReport
            {
                MainReport = mainReports.ToList(),
                TotalOrdersHandled = totalOrderReportToInt,
                TopSellingProductInCategory = topSellingProductsToInt
            };
        }

        //public async Task<FinalPerfomanceReport> GetPerformanceReportSP(int year, string categoryName)
        //{

        //}


        public async Task<string> EmployeeMakeOrder(EmployeePlaceOrderRequest request)
        {
            //pending: deduct and update the product table after employee make order
            string sqlCheckEmployeeExist = @"SELECT EmployeeID FROM Employees WHERE EmployeeID = @EmployeeId";

            var checkEmployeeExistResult = await _db.QueryAsync<int>(sqlCheckEmployeeExist, new
            {
                EmployeeId = request.EmployeeId
            });

            if (checkEmployeeExistResult != null)
            {
                //employee exist
                //iterate through the list to check if there is enough quantity for the product id from Products table

                foreach (var employeeNewOrder in request.OrderItems)
                {
                    string sqlCheckProductQuantity = @"SELECT 
                                                   CASE
	                                                 WHEN UnitsInStock IS NULL OR UnitsInStock < @reqQuantity THEN 0
	                                                 ELSE UnitsInStock
                                                   END AS UnitStock
                                                   FROM Products
                                                   WHERE ProductID = @ProdId";

                    var checkProductQuantityResult = await _db.QueryAsync<int>(sqlCheckProductQuantity, new
                    {
                        ProdId = employeeNewOrder.ProductId,
                        reqQuantity = employeeNewOrder.Quantity
                    });

                    if (checkProductQuantityResult.FirstOrDefault() != 0)
                    {
                        //product is available
                        //get the unit price product details first
                        string sqlGetProductDetails = @"SELECT UnitPrice FROM Products WHERE ProductID=@ProdId";

                        var getProductDetailsResult = await _db.QueryAsync<decimal>(sqlGetProductDetails, new
                        {
                            ProdId = employeeNewOrder.ProductId
                        });

                        decimal UnitPrice = getProductDetailsResult.FirstOrDefault();

                        //create the Orders
                        string sqlInsertDataIntoProducts = @"
                                                           INSERT INTO 
                                                           Orders
                                                           (EmployeeID,OrderDate) 
                                                           VALUES
                                                           (@EmpId,@OrderDate);
                                                            SELECT SCOPE_IDENTITY() AS NewProductID;";

                        var insertDataIntoProductsResult = await _db.QueryAsync<int>(sqlInsertDataIntoProducts, new
                        {
                            //CustId = request.EmployeeId,
                            EmpId = request.EmployeeId,
                            OrderDate = DateTime.Now

                        });

                        int OrderIdAfterInsert = insertDataIntoProductsResult.FirstOrDefault();


                        //insert into [Order Details] Table
                        string sqlInsertIntoOrderDetails = @"INSERT INTO
                                                             [Order Details]
                                                             (OrderID,ProductID,UnitPrice,Quantity)
                                                             VALUES
                                                             (@OrderId,@ProductId,@UnitPrice,@Quantity)";

                        var insertIntoOrderDetailsResult = await _db.QueryAsync(sqlInsertIntoOrderDetails, new
                        {
                            OrderId = OrderIdAfterInsert,
                            ProductId = employeeNewOrder.ProductId,
                            UnitPrice = UnitPrice,
                            Quantity = employeeNewOrder.Quantity
                        });

                    }
                    else
                    {
                        //the product is not available
                        //end the transaction
                        return "Product stock not available(Product ID:" + employeeNewOrder.ProductId + ")";
                    }

                }

            }
            else
            {
                //employee doesnt exist
                //return null
                return "Employee Verification Failed(Employee Id:" + request.EmployeeId + ")";

            }

            return "Order Inserted Successfully";

        }

    }
}
