using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CampusService
{
    
    public class Service1 : IService1
    {

        DataClasses1DataContext db = new DataClasses1DataContext();

     
     
        public int AddProduct(Product p)
        {
            try
            {
                //add a new product
                db.Products.InsertOnSubmit(p);
                db.SubmitChanges();
                return 0;
            }catch(Exception e)
            {
                e.GetBaseException();
                return -1;
            }
        }

     

        public Product GetProduct(int id)
        {
            //check existing ones
            var product = (from p in db.Products where p.Id.Equals(id) select p).FirstOrDefault();

            return product;

        }

        public List<Product> GetProducts()
        {
            dynamic product = (from p in db.Products select p).DefaultIfEmpty();
            List<Product> products =  null;

            if(product != null)
            {
                foreach(Product pd in product)
                {
                    products.Add(pd);
                }
                
            }
            return products;
        }

        public User getUser(string password, string email)
        {
            var user = (from u in db.Users
                        where u.password == password && u.email == email
                        select u).FirstOrDefault();

            if (user == null)
            {
                return null; // safer than throwing an exception
            }

            return new User
            {
                Id = user.Id,
                name = user.name,
                email = user.email,
                password = user.password
                // Don't return navigation properties!
            };
        }


        public Product GetProductByUserEmail(string email)
        {
            var user = (from u in db.Users where u.email.Equals(email) select u).FirstOrDefault();


            var product = (from p in db.Products where p.userId.Equals(user.Id) select p).FirstOrDefault();
            if (product == null)
                return null;

            return product;
        }

            public int Login(string email, string password)
        {
            try
            {
                var user = (from u in db.Users where email == u.email && password == u.password select u).FirstOrDefault();

                //user doesnt exist yet
                if (user == null)
                    return 0;
                //user exists
                else
                    return 1;

            }
            catch (Exception e)
            {
                //something went wrong
                e.GetBaseException();
               return -1;

            }
        }


        //register section
            public int Register(User argUser)
            {
              var user = (from u in db.Users where u.email.Equals(argUser.email) && u.name.Equals(argUser.name) && u.password.Equals(argUser.password) select u).FirstOrDefault();
                                  
            try
            {
                 //user exists
                if (user != null)
                    return 1;

                //user doesnt exists
                else {
      
                    db.Users.InsertOnSubmit(argUser);
                    db.SubmitChanges();
                    return 0;
            }
            }
            catch (Exception e)
            {
                e.GetBaseException();
                return -1;
            }
           }
           using System;
 public int GetProductsSoldCount(DateTime startDate, DateTime endDate)
        {
            try
            {
                var count = (from il in db.InvoiceLines
                            join i in db.Invoices on il.invoiceId equals i.Id
                            where i.date >= startDate && i.date <= endDate
                            select il.productId).Distinct().Count();
                
                return count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetProductsSoldCount: " + ex.Message);
                return 0;
            }
        }

        public int GetTotalStockCount()
        {
            try
            {
                return (from p in db.Products where p.active == 1 select p.quatity).Sum();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetTotalStockCount: " + ex.Message);
                return 0;
            }
        }

        public int GetDailyRegistrations(DateTime date)
        {
            try
            {
                return (from u in db.Users 
                       where u.registrationDate.HasValue && 
                             u.registrationDate.Value.Date == date.Date 
                       select u).Count();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetDailyRegistrations: " + ex.Message);
                return 0;
            }
        }

        public decimal GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            try
            {
                var revenue = (from i in db.Invoices
                             where i.date >= startDate && i.date <= endDate
                             select i.totalPrice).Sum();
                
                return revenue ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetTotalRevenue: " + ex.Message);
                return 0;
            }
        }

        public DataTable GetSalesByCategory(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("TotalSales", typeof(decimal));
            dt.Columns.Add("NumberOfOrders", typeof(int));
            dt.Columns.Add("QuantitySold", typeof(int));

            try
            {
                var salesData = (from il in db.InvoiceLines
                                join i in db.Invoices on il.invoiceId equals i.Id
                                join p in db.Products on il.productId equals p.Id
                                where i.date >= startDate && i.date <= endDate
                                group new { il, p } by p.category into g
                                select new
                                {
                                    Category = g.Key,
                                    TotalSales = g.Sum(x => x.il.price * x.il.quantity),
                                    NumberOfOrders = g.Select(x => x.il.invoiceId).Distinct().Count(),
                                    QuantitySold = g.Sum(x => x.il.quantity)
                                }).ToList();

                foreach (var item in salesData)
                {
                    dt.Rows.Add(item.Category, item.TotalSales, item.NumberOfOrders, item.QuantitySold);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetSalesByCategory: " + ex.Message);
            }

            return dt;
        }

        public DataTable GetLowStockProducts(int threshold)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("CurrentStock", typeof(int));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));

            try
            {
                var lowStockProducts = (from p in db.Products
                                       where p.quatity <= threshold && p.active == 1
                                       select p).ToList();

                foreach (var product in lowStockProducts)
                {
                    dt.Rows.Add(product.name, product.quatity, product.category, product.price);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetLowStockProducts: " + ex.Message);
            }

            return dt;
        }

        public DataTable GetStockByCategory()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("TotalStock", typeof(int));
            dt.Columns.Add("NumberOfProducts", typeof(int));
            dt.Columns.Add("AveragePrice", typeof(decimal));

            try
            {
                var stockData = (from p in db.Products
                                where p.active == 1
                                group p by p.category into g
                                select new
                                {
                                    Category = g.Key,
                                    TotalStock = g.Sum(x => x.quatity),
                                    NumberOfProducts = g.Count(),
                                    AveragePrice = g.Average(x => x.price)
                                }).ToList();

                foreach (var item in stockData)
                {
                    dt.Rows.Add(item.Category, item.TotalStock, item.NumberOfProducts, item.AveragePrice);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetStockByCategory: " + ex.Message);
            }

            return dt;
        }

        public int GetActiveUsersCount(DateTime startDate, DateTime endDate)
        {
            try
            {
                return (from i in db.Invoices
                       where i.date >= startDate && i.date <= endDate
                       select i.userId).Distinct().Count();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetActiveUsersCount: " + ex.Message);
                return 0;
            }
        }

        public decimal GetTopSpenderAmount(DateTime startDate, DateTime endDate)
        {
            try
            {
                var topSpender = (from i in db.Invoices
                                 where i.date >= startDate && i.date <= endDate
                                 group i by i.userId into g
                                 select new
                                 {
                                     TotalSpent = g.Sum(x => x.totalPrice)
                                 }).OrderByDescending(x => x.TotalSpent).FirstOrDefault();

                return topSpender?.TotalSpent ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetTopSpenderAmount: " + ex.Message);
                return 0;
            }
        }

        public decimal GetAverageUserSpend(DateTime startDate, DateTime endDate)
        {
            try
            {
                var userSpending = (from i in db.Invoices
                                   where i.date >= startDate && i.date <= endDate
                                   group i by i.userId into g
                                   select g.Sum(x => x.totalPrice)).ToList();

                return userSpending.Count > 0 ? userSpending.Average() : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetAverageUserSpend: " + ex.Message);
                return 0;
            }
        }

        public int GetActiveAuctionsCount()
        {
            try
            {
                return (from p in db.Products
                       where p.active == 1 && p.dateBid > DateTime.Now
                       select p).Count();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetActiveAuctionsCount: " + ex.Message);
                return 0;
            }
        }

        public int GetCompletedAuctionsCount(DateTime startDate, DateTime endDate)
        {
            try
            {
                return (from p in db.Products
                       where p.dateBid >= startDate && p.dateBid <= endDate && p.dateBid <= DateTime.Now
                       select p).Count();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetCompletedAuctionsCount: " + ex.Message);
                return 0;
            }
        }

        public decimal GetAverageWinningBid(DateTime startDate, DateTime endDate)
        {
            try
            {
                var completedAuctions = (from p in db.Products
                                        where p.dateBid >= startDate && p.dateBid <= endDate && 
                                              p.dateBid <= DateTime.Now && p.active == 1
                                        select p.price).ToList();

                return completedAuctions.Count > 0 ? completedAuctions.Average() : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetAverageWinningBid: " + ex.Message);
                return 0;
            }
        }

        public double GetAuctionSuccessRate(DateTime startDate, DateTime endDate)
        {
            try
            {
                var totalAuctions = (from p in db.Products
                                    where p.dateBid >= startDate && p.dateBid <= endDate
                                    select p).Count();

                var successfulAuctions = (from p in db.Products
                                         where p.dateBid >= startDate && p.dateBid <= endDate && 
                                               p.dateBid <= DateTime.Now && p.active == 1
                                         select p).Count();

                return totalAuctions > 0 ? (successfulAuctions * 100.0 / totalAuctions) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetAuctionSuccessRate: " + ex.Message);
                return 0;
            }
        }

        public DataTable GetDailyRegistrationsReport(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Registrations", typeof(int));

            try
            {
                // Note: You'll need to add a registrationDate field to your User table
                var registrations = (from u in db.Users
                                    where u.registrationDate >= startDate && u.registrationDate <= endDate
                                    group u by u.registrationDate.Value.Date into g
                                    select new
                                    {
                                        Date = g.Key,
                                        Count = g.Count()
                                    }).ToList();

                foreach (var item in registrations)
                {
                    dt.Rows.Add(item.Date.ToString("yyyy-MM-dd"), item.Count);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetDailyRegistrationsReport: " + ex.Message);
            }

            return dt;
        }

        // CART AND ORDER METHODS (add these too)

        public int AddToCart(int userId, int productId, int quantity)
        {
            try
            {
                var existingItem = (from c in db.Carts
                                   where c.userId == userId && c.productId == productId
                                   select c).FirstOrDefault();

                if (existingItem != null)
                {
                    existingItem.quantity += quantity;
                }
                else
                {
                    Cart newItem = new Cart
                    {
                        userId = userId,
                        productId = productId,
                        quantity = quantity,
                        addedDate = DateTime.Now
                    };
                    db.Carts.InsertOnSubmit(newItem);
                }

                db.SubmitChanges();
                return 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in AddToCart: " + ex.Message);
                return -1;
            }
        }

        public List<Cart> GetUserCart(int userId)
        {
            try
            {
                return (from c in db.Carts
                       where c.userId == userId
                       select c).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetUserCart: " + ex.Message);
                return new List<Cart>();
            }
        }
    }
}
