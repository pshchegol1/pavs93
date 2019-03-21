
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Additional namespaces
using northwindSystem.BLL;
using NorthwindSystem.Data;
#endregion

namespace WebApp.SamplePages
{
    public partial class SimpleQueries : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear old messages
            MessageLabel.Text = "";
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            int productid = 0;
            //Validate your input
            if (string.IsNullOrEmpty(SearchArg.Text.Trim()))
            {
                //if its bad: message to user
                MessageLabel.Text = "Product Id is required";
            }
            else if(int.TryParse(SearchArg.Text, out productid))
            {
                //if its good: Standard lookup pattern and display
                //Since we are leaving this project (webapp and going to another project (BLL)
                // User friendly error handling is required
                try
                {
                    //Create an instance of the appropriate BLL class
                    ProductController sysmgr = new ProductController();

                    //Issue your request to the appropriate BLL class method
                    Product results = sysmgr.Product_Get(int.Parse(SearchArg.Text));
                    //Test results to see if anything was found
                    //null: product id not found
                    //otherwise: product instance exists

                    if (results == null)
                    {
                        //bad: message to user
                        MessageLabel.Text = "No data found for supplied ID";
                    }
                    else
                    {
                        //good: found
                        ProductID.Text = results.ProductID.ToString();
                        ProductName.Text = results.ProductName;
                    }


                }
                catch(Exception ex)
                {
                    MessageLabel.Text = ex.Message;
                }



            }
            else
            {
                //if its bad: message to user
                MessageLabel.Text = "Product ID must be a number greater than 0";
            }
           

        }

        protected void Clear_Click(object sender, EventArgs e)
        {
            SearchArg.Text = "";
            ProductID.Text = "";
            ProductName.Text = "";
        }
    }
}