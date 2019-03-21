
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional Namespaces
using NorthwindSystem.Data;
using northwindSystem.DAL;
using System.Data.SqlClient;
#endregion

namespace northwindSystem.BLL
{
    //This class will be called from an external source
    //In our example, this source will be the web page
    //Naming standard is <T>Controller which represents a particular dat class (sql table)
    public class ProductController
    {
        //Code methods which will be called for processing
        //methods will be public
        //These methods aree referred to as the system interface

        //A method to look up a record on the database table by primary key

        //input: primary keyvalue

        //output: instance of data class
        public Product Product_Get(int productid)
        {
            //The processing of the request will be done in a transaction using context class

            //a)instance of Context class
            //b) issue the request for lookup via the appropriate DbSet<T>
            //c) Return results

            using (var context = new NorthwindContext())
            {
                return context.Products.Find(productid);
            }
        }

        //a Method to retrieve all records on the DbSet<T>
        //input: none
        //output: List<T>
        public List<Product> Product_List()
        {
            using (var context = new NorthwindContext())
            {
                return context.Products.ToList();
            }
        }
    

        //At times you will need to do a NON-PRIMARY KEY lookup
        //You will NOT  be able to use .Find(pkey)
        //You can call sql procedures via your context class
        //within your bll class method
        //Use will use .Database.SqlQuery<T>()
        //The argument(s) for SqlQuery are a) the sql procedure execute statement(as a string)
        //b) IF REQUIRED, any arguments for the procedure
        //passing the data arguments to the procedure will make use of new .SqlParameter() object
        //The SqlParameter object needs a using clause: System.Data.SqlClient
        //Sql parameter takes two arguments a) procedure parameter name, b) value to be passed
        public List<Product> Product_GetByCategory(int categoryid)
        {
            using (var context = new NorthwindContext())
            {
                //Normally you will find that data from EntityFrameWork returns as an IEnumerable<T> datatype
                //One can convert the IEnumerable<T> to a List<T> using .ToList()
                IEnumerable<Product> results =
                    context.Database.SqlQuery<Product>("Products_GetByCategories @CategoryID", new SqlParameter("CategoryID",categoryid));
                return results.ToList();
            }
        }
    }
}
