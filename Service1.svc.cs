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
    }
}
