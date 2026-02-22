SELECT FirstName+''+LastName AS EmployeeFullName FROM Employees WHERE EmployeeID IN (SELECT EmployeeID FROM Orders WHERE YEAR(OrderDate)=1996 )
SELECT * from Orders WHERE YEAR(OrderDate)=1996

SELECT Count(OrderID) FROM Orders WHERE YEAR(OrderDate)=1996

--RevenueWithoutDiscount and DiscountAmount result is as expected(done) 
SELECT OrderId,ProductID,
 UnitPrice * Quantity AS RevenueWithoutDiscount,
 Discount * (UnitPrice * Quantity) AS DiscountAmount
FROM [Order Details] WHERE OrderID IN (SELECT OrderId FROM Orders WHERE YEAR(OrderDate)=1996)


SELECT * FROM Products WHERE ProductId IN 
(
SELECT ProductID FROM [Order Details] 
WHERE OrderId IN (
SELECT OrderId FROM Orders WHERE YEAR(OrderDate)=1996)
);

SELECT CategoryId,CategoryName FROM Categories 
WHERE CategoryID IN (
	SELECT CategoryID FROM Products 
	WHERE ProductId IN (
		SELECT ProductID FROM [Order Details] 
		WHERE OrderId IN (
			SELECT OrderId FROM Orders 
			WHERE YEAR(OrderDate)=1996
        )
    )
);

SELECT * FROM Products WHERE CategoryID=4 AND ProductId IN (
		SELECT ProductID FROM [Order Details] 
		WHERE OrderId IN (
			SELECT OrderId FROM Orders 
			WHERE YEAR(OrderDate)=1996
        )
    );

--using year:1996 and Category:Dairy Products
--sqlQueryMain
SELECT 
 e.FirstName + e.LastName AS EmployeeFullName,
 od.UnitPrice * od.Quantity AS RevenueWithoutDiscount,
 od.Discount * (od.UnitPrice * od.Quantity) AS DiscountAmount,
 p.ProductName
 FROM Employees e
 INNER JOIN Orders o ON e.EmployeeID = o.EmployeeID
 INNER JOIN [Order Details] od ON o.OrderID = od.OrderID
 INNER JOIN Products p ON od.ProductID = p.ProductID
 INNER JOIN Categories c ON p.CategoryID = c.CategoryID
 WHERE c.CategoryName='Dairy Products' AND YEAR(o.OrderDate)=1996


SELECT 
COUNT(o.OrderID) AS TotalOrdersHandled
FROM Employees e
INNER JOIN Orders o ON e.EmployeeID = o.EmployeeID
INNER JOIN [Order Details] od ON o.OrderID = od.OrderID
INNER JOIN Products p ON od.ProductID = p.ProductID
INNER JOIN Categories c ON p.CategoryID = c.CategoryID
 WHERE c.CategoryName='Dairy Products' AND YEAR(o.OrderDate)=1996          
 
SELECT 
 MAX(od.Quantity) AS TopSellingProductInCategory
 FROM Employees e
 INNER JOIN Orders o ON e.EmployeeID = o.EmployeeID
 INNER JOIN [Order Details] od ON o.OrderID = od.OrderID
 INNER JOIN Products p ON od.ProductID = p.ProductID
 INNER JOIN Categories c ON p.CategoryID = c.CategoryID
 WHERE c.CategoryName='Dairy Products' AND YEAR(o.OrderDate)=1996   


 --after inserting 
   SELECT * FROM Products WHERE ProductID IN (
  SELECT ProductID FROM [Order Details] WHERE OrderId IN
  (SELECT OrderID FROM Orders WHERE EmployeeID=5 AND YEAR(OrderDate)=2026)

  );

  SELECT * FROM [Order Details] WHERE OrderId IN
  (SELECT OrderID FROM Orders WHERE EmployeeID=5 AND YEAR(OrderDate)=2026)