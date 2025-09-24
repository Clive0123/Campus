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



}
}
