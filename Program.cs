namespace ReportGenerator
{
    class QuarterlyIncomeReport
    {
        static void Main(string[] args)
        {
            // create a new instance of the class
            QuarterlyIncomeReport report = new QuarterlyIncomeReport();

            // call the GenerateSalesData method
            SalesData[] salesData = report.GenerateSalesData();


            // call the QuarterlySalesReport method
            report.QuarterlySalesReport(salesData);

        }

        /* public struct SalesData includes the following fields: date sold, department name, product ID, quantity sold, unit price */
        public struct SalesData
        {
            public DateOnly dateSold;
            public string departmentName;
            public string productID;
            public int quantitySold;
            public double unitPrice;
            public double baseCost;
            public int volumeDiscount;
        }

        public struct ProdDepartments
        {
            public static string[] departmentNames = { "Men's Clothing", "Women's Clothing", "Children's Clothing", "Accessories", "Footwear", "Outerwear", "Sportswear", "Undergarments" };
            public static string[] departmentAbbreviations = { "MENS", "WOMN", "CHLD", "ACCS", "FOOT", "OUTR", "SPRT", "UNDR" };
        }

        public struct ManufacturingSites
        {
            public static string[] manufacturingSites = { "US1", "US2", "US3", "UK1", "UK2", "UK3", "JP1", "JP2", "JP3", "CA1" };
        }

        /* the GenerateSalesData method returns 1000 SalesData records. It assigns random values to each field of the data structure */
        public SalesData[] GenerateSalesData()
        {
            SalesData[] salesData = new SalesData[1000];
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                salesData[i].dateSold = new DateOnly(2023, random.Next(1, 13), random.Next(1, 29));
                salesData[i].departmentName = ProdDepartments.departmentNames[random.Next(0, ProdDepartments.departmentNames.Length)];
                
                salesData[i].productID = ConstructProductID(salesData[i]);
                salesData[i].quantitySold = random.Next(1, 101);
                salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                salesData[i].baseCost = salesData[i].unitPrice * (1 - (random.Next(5, 21) / 100.0));
                salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.1);

            }

            return salesData;
        }

        //Create a method called ContructProductID that takes in a SalesData struct and returns a product ID
        public string ConstructProductID(SalesData salesData)
        {
            int indexOfDept = Array.IndexOf(ProdDepartments.departmentNames, salesData.departmentName);
            string deptAbb = ProdDepartments.departmentAbbreviations[indexOfDept];
            string firstDigit = (indexOfDept + 1).ToString();
            string nextTwoDigits = new Random().Next(1, 100).ToString("D2");
            string sizeCode = new string[] { "XS", "S", "M", "L", "XL" }[new Random().Next(0, 5)];
            string colorCode = new string[] { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" }[new Random().Next(0, 8)];
            string manufacturingSite = ManufacturingSites.manufacturingSites[new Random().Next(0, ManufacturingSites.manufacturingSites.Length)];

            return $"{deptAbb}-{firstDigit}{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
        }
        public string[,] DeconstructProductID(string productID)
        {
            string[] components = productID.Split('-');
            string deptAbb = components[0];
            string productSerialNumber = components[1];
            string sizeCode = components[2];
            string colorCode = components[3];
            string manufacturingSite = components[4];

            string[,] result = new string[5, 2]
            {
                { "Department Abbreviation", deptAbb },
                { "Product Serial Number", productSerialNumber },
                { "Size Code", sizeCode },
                { "Color Code", colorCode },
                { "Manufacturing Site", manufacturingSite }
            };

            return result;
        }

        public void QuarterlySalesReport(SalesData[] salesData)
        {
            // create a dictionary to store the quarterly sales data
            Dictionary<string, double> quarterlySales = new Dictionary<string, double>();
            Dictionary<string, double> quarterlyProfit = new Dictionary<string, double>();
            Dictionary<string, double> quarterlyProfitPercentage = new Dictionary<string, double>();

            // create a dictionary to store the quarterly sales data by department
            Dictionary<string, Dictionary<string, double>> quarterlySalesByDepartment = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> quarterlyProfitByDepartment = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> quarterlyProfitPercentageByDepartment = new Dictionary<string, Dictionary<string, double>>();

            // create a dictionary to store the top 3 sales orders by quarter
            Dictionary<string, List<SalesData>> top3SalesOrdersByQuarter = new Dictionary<string, List<SalesData>>();

            // create a dictionary to store detailed sales data by product ID for each quarter
            Dictionary<string, Dictionary<string, (int unitsSold, double totalSales, double avgUnitCost, double totalProfit, double avgProfitPercentage)>> quarterlySalesByProductID = new Dictionary<string, Dictionary<string, (int, double, double, double, double)>>();

            // iterate through the sales data
            foreach (SalesData data in salesData)
            {
                // calculate the total sales for each quarter
                string quarter = GetQuarter(data.dateSold.Month);
                double totalSales = data.quantitySold * data.unitPrice;
                double totalCost = data.quantitySold * data.baseCost;
                double profit = totalSales - totalCost;
                double profitPercentage = (profit / totalSales) * 100;

                // calculate the total sales, profit, and profit percentage by department
                if (!quarterlySalesByDepartment.ContainsKey(quarter))
                {
                    quarterlySalesByDepartment.Add(quarter, new Dictionary<string, double>());
                    quarterlyProfitByDepartment.Add(quarter, new Dictionary<string, double>());
                    quarterlyProfitPercentageByDepartment.Add(quarter, new Dictionary<string, double>());
                }

                if (quarterlySalesByDepartment[quarter].ContainsKey(data.departmentName))
                {
                    quarterlySalesByDepartment[quarter][data.departmentName] += totalSales;
                    quarterlyProfitByDepartment[quarter][data.departmentName] += profit;
                }
                else
                {
                    quarterlySalesByDepartment[quarter].Add(data.departmentName, totalSales);
                    quarterlyProfitByDepartment[quarter].Add(data.departmentName, profit);
                }

                if (!quarterlyProfitPercentageByDepartment[quarter].ContainsKey(data.departmentName))
                {
                    quarterlyProfitPercentageByDepartment[quarter].Add(data.departmentName, profitPercentage);
                }

                // calculate the total sales and profit for each quarter
                if (quarterlySales.ContainsKey(quarter))
                {
                    quarterlySales[quarter] += totalSales;
                    quarterlyProfit[quarter] += profit;
                }
                else
                {
                    quarterlySales.Add(quarter, totalSales);
                    quarterlyProfit.Add(quarter, profit);
                }

                if (!quarterlyProfitPercentage.ContainsKey(quarter))
                {
                    quarterlyProfitPercentage.Add(quarter, profitPercentage);
                }

                // add the sales data to the top 3 sales orders by quarter
                if (!top3SalesOrdersByQuarter.ContainsKey(quarter))
                {
                    top3SalesOrdersByQuarter.Add(quarter, new List<SalesData>());
                }

                top3SalesOrdersByQuarter[quarter].Add(data);

                // Deconstruct the product ID
                string[,] productIDComponents = DeconstructProductID(data.productID);

                // Create a new string with the department abbreviation and product serial number
                string newProductID = $"{productIDComponents[0, 1]}-{productIDComponents[1, 1]}-ss-cc-mmm";

                // Add the detailed sales data by product ID for each quarter
                if (!quarterlySalesByProductID.ContainsKey(quarter))
                {
                    quarterlySalesByProductID.Add(quarter, new Dictionary<string, (int, double, double, double, double)>());
                }

                if (quarterlySalesByProductID[quarter].ContainsKey(newProductID))
                {
                    var existingData = quarterlySalesByProductID[quarter][newProductID];
                    existingData.unitsSold += data.quantitySold;
                    existingData.totalSales += totalSales;
                    existingData.totalProfit += profit;
                    existingData.avgUnitCost = existingData.totalSales / existingData.unitsSold;
                    existingData.avgProfitPercentage = (existingData.totalProfit / existingData.totalSales) * 100;
                    quarterlySalesByProductID[quarter][newProductID] = existingData;
                }
                else
                {
                    quarterlySalesByProductID[quarter].Add(newProductID, (data.quantitySold, totalSales, totalSales / data.quantitySold, profit, profitPercentage));
                }
            }

            // sort the top 3 sales orders by profit in descending order
            foreach (var quarter in top3SalesOrdersByQuarter.Keys)
            {
                top3SalesOrdersByQuarter[quarter] = top3SalesOrdersByQuarter[quarter]
                    .OrderByDescending(order => (order.quantitySold * order.unitPrice) - (order.quantitySold * order.baseCost))
                    .Take(3)
                    .ToList();
            }

            // sort the quarterly sales by product ID by total profit in descending order and take the top 3 for each quarter
            foreach (var quarter in quarterlySalesByProductID.Keys)
            {
                var sortedSalesByProductID = quarterlySalesByProductID[quarter]
                    .OrderByDescending(product => product.Value.totalProfit)
                    .Take(3)
                    .ToDictionary(product => product.Key, product => product.Value);

                quarterlySalesByProductID[quarter] = sortedSalesByProductID;
            }

            // display the quarterly sales report
            Console.WriteLine("Quarterly Sales Report");
            Console.WriteLine("----------------------");

            // sort the quarterly sales by key (quarter)
            var sortedQuarterlySales = quarterlySales.OrderBy(q => q.Key);

            foreach (KeyValuePair<string, double> quarter in sortedQuarterlySales)
            {
                // format the sales amount as currency using regional settings
                string formattedSalesAmount = quarter.Value.ToString("C", new System.Globalization.CultureInfo("en-US"));
                string formattedProfitAmount = quarterlyProfit[quarter.Key].ToString("C", new System.Globalization.CultureInfo("en-US"));
                string formattedProfitPercentage = quarterlyProfitPercentage[quarter.Key].ToString("F2");

                Console.WriteLine("{0}: Sales: {1}, Profit: {2}, Profit Percentage: {3}%", quarter.Key, formattedSalesAmount, formattedProfitAmount, formattedProfitPercentage);

                // display the quarterly sales, profit, and profit percentage by department
                Console.WriteLine("By Department:");
                var sortedQuarterlySalesByDepartment = quarterlySalesByDepartment[quarter.Key].OrderBy(d => d.Key);

                // Print table headers
                Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┐");
                Console.WriteLine("│      Department       │       Sales       │       Profit      │ Profit Percentage │");
                Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┤");

                foreach (KeyValuePair<string, double> department in sortedQuarterlySalesByDepartment)
                {
                    string formattedDepartmentSalesAmount = department.Value.ToString("C", new System.Globalization.CultureInfo("en-US"));
                    string formattedDepartmentProfitAmount = quarterlyProfitByDepartment[quarter.Key][department.Key].ToString("C", new System.Globalization.CultureInfo("en-US"));
                    string formattedDepartmentProfitPercentage = quarterlyProfitPercentageByDepartment[quarter.Key][department.Key].ToString("F2");

                    Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │", department.Key, formattedDepartmentSalesAmount, formattedDepartmentProfitAmount, formattedDepartmentProfitPercentage);
                }

                Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┘");
                Console.WriteLine();

                // display the top 3 sales orders for the quarter
                Console.WriteLine("Top 3 Sales Orders:");
                var top3SalesOrders = top3SalesOrdersByQuarter[quarter.Key];

                // Print table headers
                Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
                Console.WriteLine("│      Product ID       │   Quantity Sold   │    Unit Price     │   Total Sales     │      Profit       │ Profit Percentage │");
                Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");

                foreach (SalesData salesOrder in top3SalesOrders)
                {
                    double orderTotalSales = salesOrder.quantitySold * salesOrder.unitPrice;
                    double orderProfit = orderTotalSales - (salesOrder.quantitySold * salesOrder.baseCost);
                    double orderProfitPercentage = (orderProfit / orderTotalSales) * 100;

                    Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │ {4,17} │ {5,17} │", salesOrder.productID, salesOrder.quantitySold, salesOrder.unitPrice.ToString("C", new System.Globalization.CultureInfo("en-US")), orderTotalSales.ToString("C", new System.Globalization.CultureInfo("en-US")), orderProfit.ToString("C", new System.Globalization.CultureInfo("en-US")), orderProfitPercentage.ToString("F2"));
                }

                Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
                Console.WriteLine();

                // display the top 3 products for the quarter
                Console.WriteLine("Top 3 Profit Products:");
                var top3Products = quarterlySalesByProductID[quarter.Key];

                // Print table headers
                Console.WriteLine("┌───────────────────────┬───────────────┬───────────────────┬───────────────────┬───────────────────┐");
                Console.WriteLine("│  Product Serial No    │  Units Sold   │    Total Sales    │     Unit Cost     │   Total Profit    │");
                Console.WriteLine("├───────────────────────┼───────────────┼───────────────────┼───────────────────┼───────────────────┤");

                foreach (var product in top3Products)
                {
                    string productSerialNo = product.Key;
                    int unitsSold = product.Value.unitsSold;
                    double totalSales = product.Value.totalSales;
                    double unitCost = product.Value.avgUnitCost;
                    double totalProfit = product.Value.totalProfit;

                    Console.WriteLine("│ {0,-21} │ {1,13} │ {2,17:C} │ {3,17:C} │ {4,17:C} │", productSerialNo, unitsSold, totalSales.ToString("C", new System.Globalization.CultureInfo("en-US")), unitCost.ToString("C", new System.Globalization.CultureInfo("en-US")), totalProfit.ToString("C", new System.Globalization.CultureInfo("en-US")));
                }

                Console.WriteLine("└───────────────────────┴───────────────┴───────────────────┴───────────────────┴───────────────────┘");
                Console.WriteLine();
            }
        }

        public string GetQuarter(int month)
        {
            if (month >= 1 && month <= 3)
            {
                return "Q1";
            }
            else if (month >= 4 && month <= 6)
            {
                return "Q2";
            }
            else if (month >= 7 && month <= 9)
            {
                return "Q3";
            }
            else
            {
                return "Q4";
            }
        }
    }
}
