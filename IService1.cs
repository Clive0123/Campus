using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CampusService
{
   
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]

        Product GetProduct( int id);
        [OperationContract]
        User getUser(string password,string email);
        [OperationContract]
        List<Product> GetProducts();

       [OperationContract]
        int Login(string email, string password);

        [OperationContract]
        int Register(User argUser);

        [OperationContract]
         int AddProduct(Product product);
        [OperationContract]
        Product GetProductByUserEmail(string email);

      [OperationContract]
        int PlaceBid(int productId, int userId, decimal bidAmount);
		
		[OperationContract]
		int GetProductsSoldCount(DateTime startDate, DateTime endDate);

		[OperationContract]
		int GetTotalStockCount();

		[OperationContract]
		int GetDailyRegistrations(DateTime date);

		[OperationContract]
		decimal GetTotalRevenue(DateTime startDate, DateTime endDate);

		[OperationContract]
		DataTable GetSalesByCategory(DateTime startDate, DateTime endDate);

		[OperationContract]
		DataTable GetLowStockProducts(int threshold);

		[OperationContract]
		DataTable GetStockByCategory();
		[OperationContract]
        int AddToCart(int userId, int productId, int quantity);

        [OperationContract]
        List<Cart> GetUserCart(int userId);

        [OperationContract]
        int RemoveFromCart(int cartId);

        [OperationContract]
        int CreateOrder(int userId, decimal totalAmount);

        [OperationContract]
        int AddOrderItem(int orderId, int productId, int quantity, decimal price);

        [OperationContract]
        List<Order> GetUserOrders(int userId);

        // Admin methods
        [OperationContract]
        List<Product> GetProductsByUserId(int userId);

        [OperationContract]
        List<Product> GetProductsForApproval();

        [OperationContract]
        int ApproveProduct(int productId);

        [OperationContract]
        int RejectProduct(int productId);
}
}
